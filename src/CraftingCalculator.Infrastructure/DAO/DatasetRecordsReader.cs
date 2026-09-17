using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO;

/// <summary>
/// Reads the categories, components and blueprints of the dataset a <see cref="CraftingDataContext"/> is scoped to,
/// with the links between them, as the flat records the model builders take.
/// </summary>
internal static class DatasetRecordsReader
{
    /// <summary>Every category, component and blueprint in the context's dataset, each list in id order.</summary>
    public static async Task<DatasetRecords> ReadAsync(CraftingDataContext context)
    {
        List<SnapshotCategory> categories = await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .Select(category => new SnapshotCategory(category.Id, category.Name, category.Description))
            .ToListAsync();

        List<SnapshotComponent> components = await context.Components
            .AsNoTracking()
            .OrderBy(component => component.Id)
            .Select(component => new SnapshotComponent(
                component.Id, component.Name, component.Description, component.Cost, component.ProductionTime,
                component.CategoryId))
            .ToListAsync();

        // The link tables are read whole and grouped in memory rather than through an Include per blueprint:
        // five queries for the whole dataset, however deep the blueprints nest.
        ILookup<int, QuantityLink> componentLinks = (await context.BlueprintComponents
                .AsNoTracking()
                .OrderBy(link => link.Id)
                .Select(link => new { link.BlueprintId, link.ComponentId, link.Quantity })
                .ToListAsync())
            .ToLookup(link => link.BlueprintId, link => new QuantityLink(link.ComponentId, link.Quantity));

        ILookup<int, QuantityLink> childLinks = (await context.BlueprintChildren
                .AsNoTracking()
                .OrderBy(link => link.Id)
                .Select(link => new { link.ParentBlueprintId, link.ChildBlueprintId, link.Quantity })
                .ToListAsync())
            .ToLookup(link => link.ParentBlueprintId, link => new QuantityLink(link.ChildBlueprintId, link.Quantity));

        List<Blueprint> blueprints = await context.Blueprints.AsNoTracking().OrderBy(blueprint => blueprint.Id).ToListAsync();

        return new DatasetRecords(
            categories,
            components,
            [
                .. blueprints.Select(blueprint => new SnapshotBlueprint(
                    blueprint.Id, blueprint.Name, blueprint.Description, blueprint.Value, blueprint.Yield,
                    blueprint.ProductionTime, blueprint.CategoryId,
                    [.. componentLinks[blueprint.Id]], [.. childLinks[blueprint.Id]]))
            ]);
    }
}
