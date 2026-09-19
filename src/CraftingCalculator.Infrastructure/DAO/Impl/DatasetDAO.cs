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

    public async Task<DatasetModel> CopyAsync(int sourceId, string name) =>
        await ImportAsNewAsync(name, await GetSnapshotAsync(sourceId));

    public async Task<DatasetSnapshot> GetSnapshotAsync(int datasetId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Scopes every read below to this dataset, the way DatasetScopedContextFactory scopes a DAO to the selected
        // one. The query filters then do the work, so no read has to name DatasetId to keep another dataset's rows out.
        context.DatasetId = datasetId;

        Dataset dataset = await context.Datasets.AsNoTracking().SingleAsync(dataset => dataset.Id == datasetId);

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
            dataset.Name,
            records.Categories,
            records.Components,
            records.Blueprints,
            [
                .. favorites.Select(favorite => new SnapshotFavorite(
                    favorite.Id, favorite.Name, [.. favoriteLinks[favorite.Id]]))
            ],
            ToSettings(dataset));
    }

    public async Task<DatasetModel> ImportAsNewAsync(string name, DatasetSnapshot snapshot)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // The records carry the new dataset's id, so its row has to be inserted before them: two SaveChanges calls,
        // which is what the transaction makes one unit. Without it a failure partway through the records would leave a
        // half-filled dataset behind.
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Dataset dataset = new() { Name = name };
        ApplySettings(dataset, snapshot.Settings);
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

    public async Task SetSettingsAsync(int id, Datasettings settings)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Dataset dataset = await context.Datasets.SingleAsync(dataset => dataset.Id == id);
        ApplySettings(dataset, settings);
        await context.SaveChangesAsync();
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
    /// Stages every record of <paramref name="plan"/>'s incoming snapshot into the context's dataset, landing each on
    /// the row the plan gives it, with every link pointing at the row its target landed on.
    /// </summary>
    private static async Task StageAsync(CraftingDataContext context, MergePlan plan)
    {
        Landing landing = new(plan);

        // Ordered by what a record points at: a component is filed under a category, a blueprint uses components, and a
        // favorite holds blueprints. Each step hands the next a map from incoming id to the row that record landed on.
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

            if (!write) continue;
            row.Name = record.Name;
            row.Description = record.Description;
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
        Name = entity.Name,
        Settings = ToSettings(entity)
    };

    // The two directions of the one mapping between the Datasets row and Datasettings, so a new setting is added to both
    // in the same place. Named arguments, because settings of the same type would otherwise transpose silently.
    private static Datasettings ToSettings(Dataset entity) =>
        new(UseYield: entity.UseYield, UseCosts: entity.UseCosts, UseValues: entity.UseValues,
            UseCraftTime: entity.UseCraftTime);

    private static void ApplySettings(Dataset entity, Datasettings settings)
    {
        entity.UseYield = settings.UseYield;
        entity.UseCosts = settings.UseCosts;
        entity.UseValues = settings.UseValues;
        entity.UseCraftTime = settings.UseCraftTime;
    }
}
