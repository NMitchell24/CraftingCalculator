using System.Text.Json;
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
        // The link tables are read whole and grouped in memory rather than through an Include per blueprint:
        // five queries for the whole dataset, however deep the blueprints nest.
        List<LinkRow> childLinks = await ReadChildLinksAsync(context.BlueprintChildren);
        List<LinkRow> componentLinks = await ReadComponentLinksAsync(context.BlueprintComponents);

        return await AssembleAsync(
            context.Categories, context.Components, context.Blueprints, componentLinks, childLinks);
    }

    /// <summary>
    /// The blueprints with the given <paramref name="blueprintIds"/> and every record they reach through their parts at
    /// any depth, each list in id order. An id the context's dataset has no blueprint for is left out.
    /// </summary>
    public static async Task<DatasetRecords> ReadBlueprintTreesAsync(CraftingDataContext context, IReadOnlyCollection<int> blueprintIds)
    {
        // One recursive query for every tree, whatever their depth, seeded with the roots through json_each so the
        // ids go in as one parameter. UNION rather than UNION ALL is what ends the recursion on a cyclic row set, and
        // the join on both ends' DatasetId keeps the walk from following a link out of the dataset and back in. The
        // SQL opens with SELECT so EF can compose over it, which is how the dataset's query filter still applies to
        // the rows it returns.
        string roots = JsonSerializer.Serialize(blueprintIds);
        List<LinkRow> childLinks = await ReadChildLinksAsync(context.BlueprintChildren.FromSql(
            $"""
             SELECT * FROM BlueprintChildren WHERE ParentBlueprintId IN (
                 WITH RECURSIVE Tree(Id) AS (
                     SELECT value FROM json_each({roots})
                     UNION
                     SELECT link.ChildBlueprintId
                     FROM BlueprintChildren AS link
                     JOIN Tree ON link.ParentBlueprintId = Tree.Id
                     JOIN Blueprints AS parent ON parent.Id = link.ParentBlueprintId
                     JOIN Blueprints AS child ON child.Id = link.ChildBlueprintId AND child.DatasetId = parent.DatasetId)
                 SELECT Id FROM Tree)
             """));

        HashSet<int> reached = [.. blueprintIds, .. childLinks.Select(link => link.TargetId)];

        List<LinkRow> componentLinks = await ReadComponentLinksAsync(
            context.BlueprintComponents.Where(link => reached.Contains(link.BlueprintId)));

        List<int> componentIds = [.. componentLinks.Select(link => link.TargetId).Distinct()];

        return await AssembleAsync(
            context.Categories,
            context.Components.Where(component => componentIds.Contains(component.Id)),
            context.Blueprints.Where(blueprint => reached.Contains(blueprint.Id)),
            componentLinks,
            childLinks);
    }

    // Nothing in the schema stops a link from naming a record in another dataset, which the dataset's own record
    // queries would then leave out and the model builder would fail to resolve. Such a link is dropped here, the
    // way the builder drops a cyclic one, so one bad row cannot fail every screen that reads a blueprint.
    private static Task<List<LinkRow>> ReadChildLinksAsync(IQueryable<BlueprintChild> links) =>
        links.AsNoTracking()
            .Where(link => link.Child.DatasetId == link.ParentBlueprint.DatasetId)
            .Select(link => new LinkRow(link.Id, link.ParentBlueprintId, link.ChildBlueprintId, link.Quantity))
            .ToListAsync();

    private static Task<List<LinkRow>> ReadComponentLinksAsync(IQueryable<BlueprintComponent> links) =>
        links.AsNoTracking()
            .Where(link => link.Component.DatasetId == link.Blueprint.DatasetId)
            .Select(link => new LinkRow(link.Id, link.BlueprintId, link.ComponentId, link.Quantity))
            .ToListAsync();

    // Categories are read whole whichever records are asked for: there are few of them, and a component's or a
    // blueprint's category is only known once both have been read.
    private static async Task<DatasetRecords> AssembleAsync(
        IQueryable<Category> categoryQuery,
        IQueryable<Component> componentQuery,
        IQueryable<Blueprint> blueprintQuery,
        List<LinkRow> componentLinks,
        List<LinkRow> childLinks)
    {
        List<SnapshotCategory> categories = await categoryQuery
            .AsNoTracking()
            .OrderBy(category => category.Id)
            .Select(category => new SnapshotCategory(category.Id, category.Name, category.Description))
            .ToListAsync();

        List<SnapshotComponent> components = await componentQuery
            .AsNoTracking()
            .OrderBy(component => component.Id)
            .Select(component => new SnapshotComponent(
                component.Id, component.Name, component.Description, component.Cost, component.ProductionTime,
                component.CategoryId))
            .ToListAsync();

        List<Blueprint> blueprints = await blueprintQuery.AsNoTracking().OrderBy(blueprint => blueprint.Id).ToListAsync();

        ILookup<int, QuantityLink> componentsByBlueprint = ToLookup(componentLinks);
        ILookup<int, QuantityLink> childrenByBlueprint = ToLookup(childLinks);

        return new DatasetRecords(
            categories,
            components,
            [
                .. blueprints.Select(blueprint => new SnapshotBlueprint(
                    blueprint.Id, blueprint.Name, blueprint.Description, blueprint.Value, blueprint.Yield,
                    blueprint.ProductionTime, blueprint.CategoryId,
                    [.. componentsByBlueprint[blueprint.Id]], [.. childrenByBlueprint[blueprint.Id]]))
            ]);
    }

    // Link id order is the order the parts were written in, which is the order a blueprint lists them.
    private static ILookup<int, QuantityLink> ToLookup(List<LinkRow> links) =>
        links.OrderBy(link => link.Id).ToLookup(link => link.OwnerId, link => new QuantityLink(link.TargetId, link.Quantity));

    private sealed record LinkRow(int Id, int OwnerId, int TargetId, long Quantity);
}
