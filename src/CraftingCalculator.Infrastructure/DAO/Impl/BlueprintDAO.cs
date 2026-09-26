using System.Text.Json;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintDAO(DatasetScopedContextFactory contextFactory) : IBlueprintDAO
{
    public async Task<BlueprintModel?> GetByIdAsync(int id) => (await GetByIdsAsync([id])).GetValueOrDefault(id);

    public async Task<Dictionary<int, BlueprintModel>> GetByIdsAsync(IReadOnlyCollection<int> ids)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        DatasetRecords records = await DatasetRecordsReader.ReadBlueprintTreesAsync(context, ids);

        return SnapshotModelProcessor.ToBlueprintModels(records, ids);
    }

    public async Task<List<BlueprintSummary>> GetSummariesAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        List<Blueprint> entities = await context.Blueprints
            .AsNoTracking()
            .Include(blueprintEntity => blueprintEntity.Category)
            .ToListAsync();

        // Ordered here rather than in SQL, so names sort by .NET's culture-aware comparison, not SQLite's binary one.
        return
        [
            .. entities.Select(entity => new BlueprintSummary
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Category = entity.Category is { } categoryEntity ? CategoryDAO.ToModel(categoryEntity) : null
            }).OrderBy(blueprint => blueprint.Name)
        ];
    }

    public async Task<HashSet<int>> GetAncestorIdsAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        // One recursive query climbing from child to parent, whatever the depth. UNION rather than UNION ALL is what
        // ends the recursion on a cyclic row set, and the join on both ends' DatasetId keeps the walk from following a
        // link out of the dataset and back in. The SQL opens with SELECT so EF can compose over it, which is how the
        // dataset's query filter still applies to the rows it returns. The id goes in as JSON, like the tree read's
        // roots, because FromSql takes its arguments as object and an int argument would be boxed.
        string seed = JsonSerializer.Serialize<int[]>([id]);
        List<int> ids = await context.Blueprints.FromSql(
                $"""
                 SELECT * FROM Blueprints WHERE Id IN (
                     WITH RECURSIVE Ancestors(Id) AS (
                         SELECT value FROM json_each({seed})
                         UNION
                         SELECT link.ParentBlueprintId
                         FROM BlueprintChildren AS link
                         JOIN Ancestors ON link.ChildBlueprintId = Ancestors.Id
                         JOIN Blueprints AS parent ON parent.Id = link.ParentBlueprintId
                         JOIN Blueprints AS child ON child.Id = link.ChildBlueprintId AND child.DatasetId = parent.DatasetId)
                     SELECT Id FROM Ancestors)
                 """)
            .AsNoTracking()
            .Where(blueprintEntity => blueprintEntity.Id != id)
            .Select(blueprintEntity => blueprintEntity.Id)
            .ToListAsync();

        return [.. ids];
    }

    public async Task<int> CountAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        return await context.Blueprints.CountAsync();
    }

    public async Task<BlueprintModel> SaveAsync(BlueprintModel blueprint)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        // The link rows are deleted and rewritten below outside the change tracker, so the transaction is what
        // keeps a failure in the second SaveChanges from leaving a blueprint with no parts.
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();

        Blueprint entity = blueprint.Id > 0
            ? await context.Blueprints.FirstAsync(blueprintEntity => blueprintEntity.Id == blueprint.Id)
            : context.Blueprints.Add(new Blueprint()).Entity;

        entity.Name = blueprint.Name ?? "";
        entity.Description = blueprint.Description ?? "";
        entity.Value = blueprint.Value;
        entity.Yield = blueprint.Yield;
        entity.ProductionTime = blueprint.ProductionTime;
        entity.CategoryId = blueprint.Category?.Id;

        // Saved first: a new blueprint's link rows need its Id.
        await context.SaveChangesAsync();

        // Replaced wholesale rather than diffed, the way BlueprintFavoritesDAO.SaveAsync writes a favorite's
        // blueprints. Nothing holds a foreign key to a link row, so its id is free to change on every save.
        await context.BlueprintComponents.Where(link => link.BlueprintId == entity.Id).ExecuteDeleteAsync();
        await context.BlueprintChildren.Where(link => link.ParentBlueprintId == entity.Id).ExecuteDeleteAsync();

        context.BlueprintComponents.AddRange(blueprint.Components.ComponentList.Select(componentQuantity => new BlueprintComponent
        {
            BlueprintId = entity.Id,
            ComponentId = componentQuantity.Component.Id,
            Quantity = componentQuantity.Quantity
        }));

        context.BlueprintChildren.AddRange(blueprint.ChildBlueprints.BlueprintList.Select(blueprintQuantity => new BlueprintChild
        {
            ParentBlueprintId = entity.Id,
            ChildBlueprintId = blueprintQuantity.Blueprint.Id,
            Quantity = blueprintQuantity.Quantity
        }));

        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        blueprint.Id = entity.Id;
        return blueprint;
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        await context.Blueprints.Where(blueprintEntity => blueprintEntity.Id == id).ExecuteDeleteAsync();
    }
}
