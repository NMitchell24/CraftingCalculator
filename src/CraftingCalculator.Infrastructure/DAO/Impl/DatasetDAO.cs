using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

// The one DAO that takes the raw IDbContextFactory instead of DatasetScopedContextFactory. It manages
// the scoping mechanism, so it has to sit outside it: routing it through the scoped factory would make
// reading the dataset list depend on a dataset already being selected, and would put a cycle between
// DatasetScopedContextFactory, ISelectedDatasetState and this DAO. Datasets carries no query filter, so
// a pooled context's leftover DatasetId has no effect on these queries.
public class DatasetDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IDatasetDAO
{
    public async Task<List<DatasetModel>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<Dataset> entities = await context.Datasets
            .AsNoTracking()
            .OrderBy(dataset => dataset.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<DatasetModel?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        Dataset? entity = await context.Datasets.AsNoTracking().FirstOrDefaultAsync(dataset => dataset.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<DatasetModel?> GetByNameAsync(string name, int exceptId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // COLLATE NOCASE rather than EF.Functions.Like: LIKE would read a user-entered % or _ in the
        // name as a wildcard, and the equality this actually wants needs no escaping. EF Core does not
        // translate the StringComparison overloads of string.Equals on SQLite, so the collation is how
        // the comparison reaches SQL. NOCASE folds ASCII only, which is what SQLite offers.
        Dataset? entity = await context.Datasets
            .AsNoTracking()
            .FirstOrDefaultAsync(dataset => dataset.Id != exceptId
                                            && EF.Functions.Collate(dataset.Name, "NOCASE") == name);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<DatasetModel> AddAsync(string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dataset entity = new() { Name = name };
        context.Datasets.Add(entity);
        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task<DatasetModel> CopyAsync(int sourceId, string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Scopes every read below to the dataset being copied, the way DatasetScopedContextFactory scopes
        // a DAO to the selected one. The query filters then do the work, so nothing in the copy methods
        // has to name DatasetId to keep another dataset's rows out.
        context.DatasetId = sourceId;

        // The copied records carry the new dataset's id, so the Datasets row has to be inserted before
        // them - two SaveChanges calls, which is what the transaction makes one unit. Without it a failure
        // partway through the records would leave a half-filled dataset behind.
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Dataset copy = new() { Name = name };
        context.Datasets.Add(copy);
        await context.SaveChangesAsync();

        // Ordered by what a copy has to point at: a component is filed under a category, a blueprint uses
        // components, and a favorite holds blueprints. Each step hands the next a map from the source
        // record's id to the copy of it, which is how a link ends up pointing inside the new dataset
        // rather than back at the record it was copied from.
        Dictionary<int, Category> categories = await CopyCategoriesAsync(context, copy.Id);
        Dictionary<int, Component> components = await CopyComponentsAsync(context, copy.Id, categories);
        Dictionary<int, Blueprint> blueprints = await CopyBlueprintsAsync(context, copy.Id, categories, components);
        await CopyFavoritesAsync(context, copy.Id, blueprints);

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return ToModel(copy);
    }

    public async Task RenameAsync(int id, string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Datasets
            .Where(dataset => dataset.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(dataset => dataset.Name, name));
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Datasets.Where(dataset => dataset.Id == id).ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        return await context.Datasets.CountAsync();
    }

    /// <summary>
    /// Stages a copy of every category in the context's dataset, filed under <paramref name="datasetId"/>,
    /// and returns them keyed by the id of the category each was copied from.
    /// </summary>
    private static async Task<Dictionary<int, Category>> CopyCategoriesAsync(CraftingDataContext context, int datasetId)
    {
        List<Category> sources = await context.Categories.AsNoTracking().ToListAsync();

        Dictionary<int, Category> copies = sources.ToDictionary(category => category.Id, category => new Category
        {
            Name = category.Name,
            Description = category.Description,

            // Set rather than left to CraftingDataContext.StampDataset, which files a new record under the
            // context's own dataset - the one being copied from.
            DatasetId = datasetId
        });

        context.Categories.AddRange(copies.Values);

        return copies;
    }

    /// <summary>
    /// Stages a copy of every component in the context's dataset, filed under <paramref name="datasetId"/>
    /// and under the copy of the category it was in, and returns them keyed by the id of the component each
    /// was copied from.
    /// </summary>
    private static async Task<Dictionary<int, Component>> CopyComponentsAsync(
        CraftingDataContext context, int datasetId, Dictionary<int, Category> categories)
    {
        List<Component> sources = await context.Components.AsNoTracking().ToListAsync();

        Dictionary<int, Component> copies = sources.ToDictionary(component => component.Id, component => new Component
        {
            Name = component.Name,
            Description = component.Description,
            Cost = component.Cost,
            ProductionTime = component.ProductionTime,
            DatasetId = datasetId,

            // The navigation rather than CategoryId: the copied category has no id until this all saves,
            // and EF fills the foreign key in from the principal it was inserted with.
            Category = component.CategoryId is int categoryId ? categories[categoryId] : null
        });

        context.Components.AddRange(copies.Values);

        return copies;
    }

    /// <summary>
    /// Stages a copy of every blueprint in the context's dataset, with its component and child-blueprint
    /// links pointing at the copies in <paramref name="datasetId"/>, and returns them keyed by the id of
    /// the blueprint each was copied from.
    /// </summary>
    private static async Task<Dictionary<int, Blueprint>> CopyBlueprintsAsync(
        CraftingDataContext context,
        int datasetId,
        Dictionary<int, Category> categories,
        Dictionary<int, Component> components)
    {
        List<Blueprint> sources = await context.Blueprints.AsNoTracking().ToListAsync();

        Dictionary<int, Blueprint> copies = sources.ToDictionary(blueprint => blueprint.Id, blueprint => new Blueprint
        {
            Name = blueprint.Name,
            Description = blueprint.Description,
            Value = blueprint.Value,
            Yield = blueprint.Yield,
            ProductionTime = blueprint.ProductionTime,
            DatasetId = datasetId,
            Category = blueprint.CategoryId is int categoryId ? categories[categoryId] : null
        });

        // The link tables are read directly rather than through an Include, matching BlueprintDAO's
        // LoadGraphAsync. Their query filters reach the dataset through the parent blueprint, so these
        // arrive already scoped to the one being copied.
        foreach (BlueprintComponent link in await context.BlueprintComponents.AsNoTracking().ToListAsync())
        {
            copies[link.BlueprintId].Components.Add(new BlueprintComponent
            {
                Component = components[link.ComponentId],
                Quantity = link.Quantity
            });
        }

        // Both ends of a child link are blueprints, which is why this runs only once every copy above
        // exists: the blueprint nested inside another is as likely as not to be one further down the list.
        foreach (BlueprintChild link in await context.BlueprintChildren.AsNoTracking().ToListAsync())
        {
            copies[link.ParentBlueprintId].Children.Add(new BlueprintChild
            {
                Child = copies[link.ChildBlueprintId],
                Quantity = link.Quantity
            });
        }

        context.Blueprints.AddRange(copies.Values);

        return copies;
    }

    /// <summary>
    /// Stages a copy of every favorite in the context's dataset, filed under <paramref name="datasetId"/>,
    /// holding the copies of the blueprints the original held and the same quantity of each.
    /// </summary>
    private static async Task CopyFavoritesAsync(
        CraftingDataContext context, int datasetId, Dictionary<int, Blueprint> blueprints)
    {
        List<Favorite> sources = await context.Favorites.AsNoTracking().ToListAsync();

        Dictionary<int, Favorite> copies = sources.ToDictionary(favorite => favorite.Id, favorite => new Favorite
        {
            Name = favorite.Name,
            DatasetId = datasetId
        });

        foreach (FavoriteBlueprint link in await context.FavoriteBlueprints.AsNoTracking().ToListAsync())
        {
            copies[link.FavoriteId].FavoriteBlueprints.Add(new FavoriteBlueprint
            {
                Blueprint = blueprints[link.BlueprintId],
                Quantity = link.Quantity
            });
        }

        context.Favorites.AddRange(copies.Values);
    }

    private static DatasetModel ToModel(Dataset entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name
    };
}
