using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Builds the models the app's dialogs display from the records of a <see cref="DatasetSnapshot"/>, so a record
/// that exists only in a snapshot, such as one staged from an import file, shows the same as one read from the
/// database.
/// </summary>
public static class SnapshotModelProcessor
{
    /// <summary>The category with <paramref name="id"/>.</summary>
    public static CategoryModel ToCategoryModel(DatasetSnapshot snapshot, int id) =>
        ToCategoryModel(snapshot.Categories.First(category => category.Id == id));

    /// <summary>The component with <paramref name="id"/>, with its category.</summary>
    public static ComponentModel ToComponentModel(DatasetSnapshot snapshot, int id)
    {
        SnapshotIndex index = new(snapshot);
        return ToComponentModel(index.Components[id], index);
    }

    /// <summary>
    /// The blueprint with <paramref name="id"/>, its components and nested blueprints built out at every depth.
    /// A nested blueprint that is already one of its own ancestors is left out, so the model is always acyclic.
    /// </summary>
    public static BlueprintModel ToBlueprintModel(DatasetSnapshot snapshot, int id)
    {
        SnapshotIndex index = new(snapshot);
        return ToBlueprintModel(index.Blueprints[id], index, []);
    }

    /// <summary>
    /// The blueprints the favorite with <paramref name="id"/> holds and how many of each, built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds a blueprint.
    /// </summary>
    public static List<BlueprintQuantity> ToFavoriteBlueprints(DatasetSnapshot snapshot, int id)
    {
        SnapshotIndex index = new(snapshot);

        return
        [
            .. snapshot.Favorites.First(favorite => favorite.Id == id).Blueprints.Select(link =>
                new BlueprintQuantity(ToBlueprintModel(index.Blueprints[link.TargetId], index, []), link.Quantity, 0))
        ];
    }

    private static CategoryModel ToCategoryModel(SnapshotCategory record) => new()
    {
        Id = record.Id,
        Name = record.Name,
        Description = record.Description
    };

    private static ComponentModel ToComponentModel(SnapshotComponent record, SnapshotIndex index) => new()
    {
        Id = record.Id,
        Name = record.Name,
        Description = record.Description,
        Cost = record.Cost,
        ProductionTime = record.ProductionTime,
        Category = CategoryOf(record.CategoryId, index)
    };

    private static BlueprintModel ToBlueprintModel(SnapshotBlueprint record, SnapshotIndex index, HashSet<int> ancestors)
    {
        BlueprintModel model = new()
        {
            Id = record.Id,
            Name = record.Name,
            Description = record.Description,
            Value = record.Value,
            Yield = record.Yield,
            ProductionTime = record.ProductionTime,
            Category = CategoryOf(record.CategoryId, index)
        };

        foreach (QuantityLink link in record.Components)
        {
            model.Components.Add(ToComponentModel(index.Components[link.TargetId], index), link.Quantity);
        }

        ancestors.Add(record.Id);

        // The same guard as BlueprintDAO.BuildModel: a legacy dataset can still hold a blueprint nested inside
        // itself, and the export screen has to show it to say so rather than recurse until the stack runs out.
        foreach (QuantityLink link in record.Blueprints.Where(link => !ancestors.Contains(link.TargetId)))
        {
            model.ChildBlueprints.Add(ToBlueprintModel(index.Blueprints[link.TargetId], index, ancestors), link.Quantity);
        }

        // Popped rather than left set, so a blueprint nested by two branches of the same tree builds under both.
        ancestors.Remove(record.Id);

        return model;
    }

    private static CategoryModel? CategoryOf(int? categoryId, SnapshotIndex index) =>
        categoryId is { } id ? ToCategoryModel(index.Categories[id]) : null;

    private sealed class SnapshotIndex(DatasetSnapshot snapshot)
    {
        public Dictionary<int, SnapshotCategory> Categories { get; } = snapshot.Categories.ToDictionary(record => record.Id);

        public Dictionary<int, SnapshotComponent> Components { get; } = snapshot.Components.ToDictionary(record => record.Id);

        public Dictionary<int, SnapshotBlueprint> Blueprints { get; } = snapshot.Blueprints.ToDictionary(record => record.Id);
    }
}
