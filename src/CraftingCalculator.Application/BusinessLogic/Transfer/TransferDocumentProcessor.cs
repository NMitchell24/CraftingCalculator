using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Transfer;

/// <summary>Turns a dataset snapshot into the document an export file holds.</summary>
public static class TransferDocumentProcessor
{
    /// <summary>
    /// The export document for the <paramref name="selected"/> records of <paramref name="snapshot"/>, and its
    /// settings whatever the selection. Refs
    /// are numbered from 1 per kind, in snapshot order, so no database id reaches the file.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="selected"/> leaves out a record that a selected record depends on.
    /// </exception>
    public static TransferDocument ToDocument(
        DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, DateTimeOffset now, string appVersion)
    {
        DatasetSnapshot extract = TransferSelectionProcessor.Extract(snapshot, selected);

        Dictionary<int, int> categoryRefs = RefsFor(extract.Categories.Select(category => category.Id));
        Dictionary<int, int> componentRefs = RefsFor(extract.Components.Select(component => component.Id));
        Dictionary<int, int> blueprintRefs = RefsFor(extract.Blueprints.Select(blueprint => blueprint.Id));
        Dictionary<int, int> favoriteRefs = RefsFor(extract.Favorites.Select(favorite => favorite.Id));

        return new TransferDocument(
            TransferFormat.Name,
            TransferFormat.CurrentVersion,
            now.ToUniversalTime(),
            appVersion,
            extract.DatasetName,
            [
                .. extract.Categories
                    .Select(category => new TransferCategory(categoryRefs[category.Id], category.Name, category.Description))
            ],
            [
                .. extract.Components
                    .Select(component => new TransferComponent(
                        componentRefs[component.Id], component.Name, component.Description, component.Cost,
                        component.ProductionTime, RefOf(categoryRefs, component.CategoryId)))
            ],
            [
                .. extract.Blueprints
                    .Select(blueprint => new TransferBlueprint(
                        blueprintRefs[blueprint.Id], blueprint.Name, blueprint.Description, blueprint.Value,
                        blueprint.Yield, blueprint.ProductionTime, RefOf(categoryRefs, blueprint.CategoryId),
                        Links(componentRefs, blueprint.Components), Links(blueprintRefs, blueprint.Blueprints)))
            ],
            [
                .. extract.Favorites
                    .Select(favorite => new TransferFavorite(
                        favoriteRefs[favorite.Id], favorite.Name, Links(blueprintRefs, favorite.Blueprints)))
            ],
            new TransferDatasettings(
                UseYield: extract.Settings.UseYield, UseCosts: extract.Settings.UseCosts,
                UseValues: extract.Settings.UseValues, UseCraftTime: extract.Settings.UseCraftTime,
                CurrencyLabel: extract.Settings.CurrencyLabel));
    }

    /// <summary>Maps each of <paramref name="ids"/> to its ref, 1 up in the order given.</summary>
    private static Dictionary<int, int> RefsFor(IEnumerable<int> ids) =>
        ids.Select((id, index) => (Id: id, Ref: index + 1))
            .ToDictionary(pair => pair.Id, pair => pair.Ref);

    private static int? RefOf(Dictionary<int, int> refs, int? id) => id is { } value ? refs[value] : null;

    private static List<QuantityRef> Links(Dictionary<int, int> refs, IEnumerable<QuantityLink> links) =>
        [.. links.Select(link => new QuantityRef(refs[link.TargetId], link.Quantity))];
}
