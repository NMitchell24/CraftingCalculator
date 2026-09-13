using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Transfer;

/// <summary>Turns a dataset snapshot into the document an export file holds.</summary>
public static class TransferDocumentProcessor
{
    /// <summary>
    /// The export document for the <paramref name="selected"/> records of <paramref name="snapshot"/>. Refs
    /// are numbered from 1 per kind, in snapshot order, so no database id reaches the file.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="selected"/> leaves out a record that a selected record depends on.
    /// </exception>
    public static TransferDocument ToDocument(
        DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, DateTimeOffset now, string appVersion)
    {
        // The file has to be importable on its own, which a link to a record left out of it would prevent.
        if (!TransferSelectionProcessor.IsClosed(DependencyGraphProcessor.Build(snapshot), selected))
        {
            throw new InvalidOperationException("The selection leaves out a record that a selected record depends on.");
        }

        Dictionary<int, int> categoryRefs = RefsFor(RecordKind.Category, snapshot.Categories.Select(category => category.Id), selected);
        Dictionary<int, int> componentRefs = RefsFor(RecordKind.Component, snapshot.Components.Select(component => component.Id), selected);
        Dictionary<int, int> blueprintRefs = RefsFor(RecordKind.Blueprint, snapshot.Blueprints.Select(blueprint => blueprint.Id), selected);
        Dictionary<int, int> favoriteRefs = RefsFor(RecordKind.Favorite, snapshot.Favorites.Select(favorite => favorite.Id), selected);

        return new TransferDocument(
            TransferFormat.Name,
            TransferFormat.CurrentVersion,
            now.ToUniversalTime(),
            appVersion,
            snapshot.DatasetName,
            [
                .. snapshot.Categories
                    .Where(category => categoryRefs.ContainsKey(category.Id))
                    .Select(category => new TransferCategory(categoryRefs[category.Id], category.Name, category.Description))
            ],
            [
                .. snapshot.Components
                    .Where(component => componentRefs.ContainsKey(component.Id))
                    .Select(component => new TransferComponent(
                        componentRefs[component.Id], component.Name, component.Description, component.Cost,
                        component.ProductionTime, RefOf(categoryRefs, component.CategoryId)))
            ],
            [
                .. snapshot.Blueprints
                    .Where(blueprint => blueprintRefs.ContainsKey(blueprint.Id))
                    .Select(blueprint => new TransferBlueprint(
                        blueprintRefs[blueprint.Id], blueprint.Name, blueprint.Description, blueprint.Value,
                        blueprint.Yield, blueprint.ProductionTime, RefOf(categoryRefs, blueprint.CategoryId),
                        Links(componentRefs, blueprint.Components), Links(blueprintRefs, blueprint.Blueprints)))
            ],
            [
                .. snapshot.Favorites
                    .Where(favorite => favoriteRefs.ContainsKey(favorite.Id))
                    .Select(favorite => new TransferFavorite(
                        favoriteRefs[favorite.Id], favorite.Name, Links(blueprintRefs, favorite.Blueprints)))
            ]);
    }

    /// <summary>Maps the id of each selected record of <paramref name="kind"/> to its ref, 1 up.</summary>
    private static Dictionary<int, int> RefsFor(RecordKind kind, IEnumerable<int> ids, IReadOnlySet<RecordKey> selected) =>
        ids.Where(id => selected.Contains(new RecordKey(kind, id)))
            .Select((id, index) => (Id: id, Ref: index + 1))
            .ToDictionary(pair => pair.Id, pair => pair.Ref);

    private static int? RefOf(Dictionary<int, int> refs, int? id) => id is { } value ? refs[value] : null;

    private static List<QuantityRef> Links(Dictionary<int, int> refs, IEnumerable<QuantityLink> links) =>
        [.. links.Select(link => new QuantityRef(refs[link.TargetId], link.Quantity))];
}
