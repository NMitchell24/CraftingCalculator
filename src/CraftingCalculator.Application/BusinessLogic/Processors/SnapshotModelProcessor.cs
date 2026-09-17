using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Builds the models the app displays from the flat records of a dataset, whether they were read from the
/// database or staged from an import file, so a record shows the same either way.
/// </summary>
public static class SnapshotModelProcessor
{
    /// <summary>The category with <paramref name="id"/>.</summary>
    public static CategoryModel ToCategoryModel(DatasetSnapshot snapshot, int id) =>
        ToCategoryModel(snapshot.Categories.First(category => category.Id == id));

    /// <summary>The component with <paramref name="id"/>, with its category.</summary>
    public static ComponentModel ToComponentModel(DatasetSnapshot snapshot, int id)
    {
        SnapshotIndex index = new(Records(snapshot));
        return ToComponentModel(index.Components[id], index);
    }

    /// <summary>
    /// The blueprint with <paramref name="id"/>, its components and nested blueprints built out at every depth.
    /// A nested blueprint that is already one of its own ancestors is left out, so the model is always acyclic.
    /// </summary>
    public static BlueprintModel ToBlueprintModel(DatasetSnapshot snapshot, int id) => ToBlueprintModel(Records(snapshot), id);

    /// <summary>
    /// The blueprint with <paramref name="id"/>, built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds one.
    /// </summary>
    public static BlueprintModel ToBlueprintModel(DatasetRecords records, int id)
    {
        SnapshotIndex index = new(records);
        return ToBlueprintModel(index.Blueprints[id], index, []);
    }

    /// <summary>
    /// Every blueprint in <paramref name="records"/>, in the order the records list them, each built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds one.
    /// </summary>
    public static List<BlueprintModel> ToBlueprintModels(DatasetRecords records)
    {
        SnapshotIndex index = new(records);
        return [.. records.Blueprints.Select(record => ToBlueprintModel(record, index, []))];
    }

    /// <summary>
    /// The blueprints the favorite with <paramref name="id"/> holds and how many of each, built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds a blueprint.
    /// </summary>
    public static List<BlueprintQuantity> ToFavoriteBlueprints(DatasetSnapshot snapshot, int id)
    {
        SnapshotIndex index = new(Records(snapshot));

        return
        [
            .. snapshot.Favorites.First(favorite => favorite.Id == id).Blueprints.Select(link =>
                new BlueprintQuantity(ToBlueprintModel(index.Blueprints[link.TargetId], index, []), link.Quantity))
        ];
    }

    private static DatasetRecords Records(DatasetSnapshot snapshot) =>
        new(snapshot.Categories, snapshot.Components, snapshot.Blueprints);

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

        // Dropping a child that is already an ancestor is what keeps a cyclic row set loadable. The parts picker
        // never offers an ancestor as a child, so nothing can write one now, but a database filled in before that
        // could, and so can an import file. Recursing into it threw out of every screen that reads a blueprint,
        // which left the whole app unusable; the export screen shows such a blueprint so it can say so.
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

    private sealed class SnapshotIndex(DatasetRecords records)
    {
        public Dictionary<int, SnapshotCategory> Categories { get; } = records.Categories.ToDictionary(record => record.Id);

        public Dictionary<int, SnapshotComponent> Components { get; } = records.Components.ToDictionary(record => record.Id);

        public Dictionary<int, SnapshotBlueprint> Blueprints { get; } = records.Blueprints.ToDictionary(record => record.Id);
    }
}
