using System.Collections.Frozen;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
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

    public async Task<DatasetSnapshot> GetSnapshotAsync(int datasetId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // The same scoping CopyAsync uses: the query filters keep every read below inside this dataset.
        context.DatasetId = datasetId;

        string name = await context.Datasets
            .AsNoTracking()
            .Where(dataset => dataset.Id == datasetId)
            .Select(dataset => dataset.Name)
            .SingleAsync();

        DatasetRecords records = await DatasetRecordsReader.ReadAsync(context);

        // Read whole and grouped in memory, the way DatasetRecordsReader reads the blueprint links.
        ILookup<int, QuantityLink> favoriteLinks = (await context.FavoriteBlueprints
                .AsNoTracking()
                .OrderBy(link => link.Id)
                .Select(link => new { link.FavoriteId, link.BlueprintId, link.Quantity })
                .ToListAsync())
            .ToLookup(link => link.FavoriteId, link => new QuantityLink(link.BlueprintId, link.Quantity));

        List<Favorite> favorites = await context.Favorites.AsNoTracking().OrderBy(favorite => favorite.Id).ToListAsync();

        return new DatasetSnapshot(
            name,
            records.Categories,
            records.Components,
            records.Blueprints,
            [
                .. favorites.Select(favorite => new SnapshotFavorite(
                    favorite.Id, favorite.Name, [.. favoriteLinks[favorite.Id]]))
            ]);
    }

    public async Task<DatasetModel> ImportAsNewAsync(string name, DatasetSnapshot snapshot)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Two SaveChanges calls for the reason CopyAsync gives: the records carry the new dataset's id, so its row
        // goes in first, and the transaction keeps a failure from leaving a half-filled dataset behind.
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Dataset dataset = new() { Name = name };
        context.Datasets.Add(dataset);
        await context.SaveChangesAsync();

        // An empty dataset has nothing for an incoming record to land on, so this is a merge that adds every record.
        context.DatasetId = dataset.Id;
        await StageAsync(context, new MergePlan(snapshot, [], FrozenSet<RecordKey>.Empty));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        return ToModel(dataset);
    }

    public async Task MergeAsync(int datasetId, MergePlan plan)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Scopes the reads of the matched rows, and files every added record under this dataset through StampDataset.
        context.DatasetId = datasetId;

        await StageAsync(context, plan);

        // One SaveChanges, which runs in a transaction of its own, so a failure writes none of it.
        await context.SaveChangesAsync();
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
            Category = component.CategoryId is { } categoryId ? categories[categoryId] : null
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
            Category = blueprint.CategoryId is { } categoryId ? categories[categoryId] : null
        });

        // The link tables are read directly rather than through an Include, matching
        // DatasetRecordsReader. Their query filters reach the dataset through the parent blueprint, so these
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

    /// <summary>
    /// Stages every record of <paramref name="plan"/>'s incoming snapshot into the context's dataset, landing each on
    /// the row the plan gives it, with every link pointing at the row its target landed on.
    /// </summary>
    private static async Task StageAsync(CraftingDataContext context, MergePlan plan)
    {
        Landing landing = new(plan);

        // Ordered by what a record points at, the same as CopyAsync, and for the same reason: each step hands the next
        // a map from incoming id to the row that record landed on.
        Dictionary<int, Category> categories = await StageCategoriesAsync(context, plan.Incoming, landing);
        Dictionary<int, Component> components = await StageComponentsAsync(context, plan.Incoming, landing, categories);
        Dictionary<int, Blueprint> blueprints = await StageBlueprintsAsync(context, plan.Incoming, landing, categories, components);
        await StageFavoritesAsync(context, plan.Incoming, landing, blueprints);
    }

    private static async Task<Dictionary<int, Category>> StageCategoriesAsync(
        CraftingDataContext context, DatasetSnapshot incoming, Landing landing)
    {
        List<int> matchedIds = landing.MatchedIds(RecordKind.Category);
        Dictionary<int, Category> matched = await context.Categories
            .Where(category => matchedIds.Contains(category.Id))
            .ToDictionaryAsync(category => category.Id);
        Dictionary<int, Category> rows = [];

        foreach (SnapshotCategory record in incoming.Categories)
        {
            (Category row, bool write) = landing.Resolve(
                RecordKind.Category, record.Id, matched, () => context.Categories.Add(new Category()).Entity);
            rows[record.Id] = row;

            if (write)
            {
                row.Name = record.Name;
                row.Description = record.Description;
            }
        }

        return rows;
    }

    private static async Task<Dictionary<int, Component>> StageComponentsAsync(
        CraftingDataContext context, DatasetSnapshot incoming, Landing landing, Dictionary<int, Category> categories)
    {
        List<int> matchedIds = landing.MatchedIds(RecordKind.Component);
        Dictionary<int, Component> matched = await context.Components
            .Where(component => matchedIds.Contains(component.Id))
            .ToDictionaryAsync(component => component.Id);
        Dictionary<int, Component> rows = [];

        foreach (SnapshotComponent record in incoming.Components)
        {
            (Component row, bool write) = landing.Resolve(
                RecordKind.Component, record.Id, matched, () => context.Components.Add(new Component()).Entity);
            rows[record.Id] = row;

            if (!write)
            {
                continue;
            }

            row.Name = record.Name;
            row.Description = record.Description;
            row.Cost = record.Cost;
            row.ProductionTime = record.ProductionTime;
            row.Category = record.CategoryId is { } categoryId ? categories[categoryId] : null;

            // Nulling a navigation that was never loaded is no change as far as EF can tell, so a replaced row's key
            // is cleared directly.
            if (row.Category is null)
            {
                row.CategoryId = null;
            }
        }

        return rows;
    }

    private static async Task<Dictionary<int, Blueprint>> StageBlueprintsAsync(
        CraftingDataContext context,
        DatasetSnapshot incoming,
        Landing landing,
        Dictionary<int, Category> categories,
        Dictionary<int, Component> components)
    {
        List<int> matchedIds = landing.MatchedIds(RecordKind.Blueprint);

        // The links come along so a replaced blueprint's can be removed before the incoming ones go in.
        Dictionary<int, Blueprint> matched = await context.Blueprints
            .Include(blueprint => blueprint.Components)
            .Include(blueprint => blueprint.Children)
            .AsSplitQuery()
            .Where(blueprint => matchedIds.Contains(blueprint.Id))
            .ToDictionaryAsync(blueprint => blueprint.Id);
        Dictionary<int, Blueprint> rows = [];
        List<(SnapshotBlueprint Record, Blueprint Row)> written = [];

        foreach (SnapshotBlueprint record in incoming.Blueprints)
        {
            (Blueprint row, bool write) = landing.Resolve(
                RecordKind.Blueprint, record.Id, matched, () => context.Blueprints.Add(new Blueprint()).Entity);
            rows[record.Id] = row;

            if (!write)
            {
                continue;
            }

            row.Name = record.Name;
            row.Description = record.Description;
            row.Value = record.Value;
            row.Yield = record.Yield;
            row.ProductionTime = record.ProductionTime;
            row.Category = record.CategoryId is { } categoryId ? categories[categoryId] : null;

            // See StageComponentsAsync.
            if (row.Category is null)
            {
                row.CategoryId = null;
            }

            context.BlueprintComponents.RemoveRange(row.Components);
            context.BlueprintChildren.RemoveRange(row.Children);
            row.Components.Clear();
            row.Children.Clear();

            row.Components.AddRange(record.Components.Select(link => new BlueprintComponent
            {
                Component = components[link.TargetId],
                Quantity = link.Quantity
            }));

            written.Add((record, row));
        }

        // Both ends of a child link are blueprints, so these wait until every incoming blueprint has a row: the one
        // nested inside another is as likely as not to be further down the list.
        foreach ((SnapshotBlueprint record, Blueprint row) in written)
        {
            row.Children.AddRange(record.Blueprints.Select(link => new BlueprintChild
            {
                Child = rows[link.TargetId],
                Quantity = link.Quantity
            }));
        }

        return rows;
    }

    private static async Task StageFavoritesAsync(
        CraftingDataContext context, DatasetSnapshot incoming, Landing landing, Dictionary<int, Blueprint> blueprints)
    {
        List<int> matchedIds = landing.MatchedIds(RecordKind.Favorite);
        Dictionary<int, Favorite> matched = await context.Favorites
            .Include(favorite => favorite.FavoriteBlueprints)
            .Where(favorite => matchedIds.Contains(favorite.Id))
            .ToDictionaryAsync(favorite => favorite.Id);

        foreach (SnapshotFavorite record in incoming.Favorites)
        {
            (Favorite row, bool write) = landing.Resolve(
                RecordKind.Favorite, record.Id, matched, () => context.Favorites.Add(new Favorite()).Entity);

            if (!write)
            {
                continue;
            }

            row.Name = record.Name;

            context.FavoriteBlueprints.RemoveRange(row.FavoriteBlueprints);
            row.FavoriteBlueprints.Clear();
            row.FavoriteBlueprints.AddRange(record.Blueprints.Select(link => new FavoriteBlueprint
            {
                Blueprint = blueprints[link.TargetId],
                Quantity = link.Quantity
            }));
        }
    }

    /// <summary>Which row of the dataset each incoming record of a <see cref="MergePlan"/> lands on.</summary>
    private sealed class Landing(MergePlan plan)
    {
        private readonly Dictionary<RecordKey, int> _matchedIds = plan.Conflicts.ToDictionary(
            conflict => new RecordKey(conflict.Kind, conflict.IncomingId), conflict => conflict.ExistingId);

        /// <summary>The ids of the dataset's records of <paramref name="kind"/> that an incoming record lands on.</summary>
        public List<int> MatchedIds(RecordKind kind) =>
            [.. plan.Conflicts.Where(conflict => conflict.Kind == kind).Select(conflict => conflict.ExistingId)];

        /// <summary>
        /// The row the incoming record <paramref name="incomingId"/> of <paramref name="kind"/> lands on, and whether
        /// its fields are written to it: the matched row from <paramref name="matchedRows"/>, written only when the
        /// plan replaces it, or else a new row from <paramref name="addRow"/>.
        /// </summary>
        public (TEntity Row, bool Write) Resolve<TEntity>(
            RecordKind kind, int incomingId, Dictionary<int, TEntity> matchedRows, Func<TEntity> addRow)
        {
            RecordKey key = new(kind, incomingId);

            return _matchedIds.TryGetValue(key, out int existingId)
                ? (matchedRows[existingId], plan.Replace.Contains(key))
                : (addRow(), true);
        }
    }

    private static DatasetModel ToModel(Dataset entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name
    };
}
