using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Builds the models the app displays from the flat records of a dataset, whether they were read from the
/// database or staged from an import file, so a record shows the same either way. A component, category or
/// blueprint reached more than once in one call is one shared instance wherever it appears.
/// </summary>
public static class SnapshotModelProcessor
{
    /// <summary>The category with <paramref name="id"/>.</summary>
    public static CategoryModel ToCategoryModel(DatasetSnapshot snapshot, int id) =>
        ToCategoryModel(snapshot.Categories.First(category => category.Id == id));

    /// <summary>The component with <paramref name="id"/>, with its category.</summary>
    public static ComponentModel ToComponentModel(DatasetSnapshot snapshot, int id) =>
        new ModelBuilder(Records(snapshot)).Component(id);

    /// <summary>
    /// The blueprint with <paramref name="id"/>, its components and nested blueprints built out at every depth.
    /// A nested blueprint that is already one of its own ancestors is left out, so the model is always acyclic.
    /// </summary>
    public static BlueprintModel ToBlueprintModel(DatasetSnapshot snapshot, int id) =>
        new ModelBuilder(Records(snapshot)).Blueprint(id);

    /// <summary>
    /// Every blueprint in <paramref name="records"/>, in the order the records list them, each built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds one.
    /// </summary>
    public static List<BlueprintModel> ToBlueprintModels(DatasetRecords records)
    {
        ModelBuilder builder = new(records);
        return [.. records.Blueprints.Select(record => builder.Blueprint(record.Id))];
    }

    /// <summary>
    /// The blueprints in <paramref name="records"/> with the given <paramref name="ids"/>, by id, each built out the
    /// way <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds one. An id the records do not hold has no
    /// entry.
    /// </summary>
    public static Dictionary<int, BlueprintModel> ToBlueprintModels(DatasetRecords records, IEnumerable<int> ids)
    {
        ModelBuilder builder = new(records);
        return ids.Distinct().Where(builder.HasBlueprint).ToDictionary(id => id, builder.Blueprint);
    }

    /// <summary>
    /// The blueprints the favorite with <paramref name="id"/> holds and how many of each, built out the way
    /// <see cref="ToBlueprintModel(DatasetSnapshot, int)"/> builds a blueprint.
    /// </summary>
    public static List<BlueprintQuantity> ToFavoriteBlueprints(DatasetSnapshot snapshot, int id)
    {
        ModelBuilder builder = new(Records(snapshot));

        return
        [
            .. snapshot.Favorites.First(favorite => favorite.Id == id).Blueprints.Select(link =>
                new BlueprintQuantity(builder.Blueprint(link.TargetId), link.Quantity))
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

    private sealed class ModelBuilder(DatasetRecords records)
    {
        private readonly Dictionary<int, SnapshotCategory> _categoryRecords = records.Categories.ToDictionary(record => record.Id);
        private readonly Dictionary<int, SnapshotComponent> _componentRecords = records.Components.ToDictionary(record => record.Id);
        private readonly Dictionary<int, SnapshotBlueprint> _blueprintRecords = records.Blueprints.ToDictionary(record => record.Id);

        private readonly Dictionary<int, CategoryModel> _categories = [];
        private readonly Dictionary<int, ComponentModel> _components = [];
        private readonly Dictionary<int, BlueprintModel> _blueprints = [];
        private readonly HashSet<int> _ancestors = [];

        public bool HasBlueprint(int id) => _blueprintRecords.ContainsKey(id);

        public BlueprintModel Blueprint(int id) => Blueprint(_blueprintRecords[id]).Model;

        public ComponentModel Component(int id)
        {
            if (_components.TryGetValue(id, out ComponentModel? model))
            {
                return model;
            }

            SnapshotComponent record = _componentRecords[id];
            model = new ComponentModel
            {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                Cost = record.Cost,
                ProductionTime = record.ProductionTime,
                Category = CategoryOf(record.CategoryId)
            };
            _components[id] = model;

            return model;
        }

        // Acyclic is true when nothing in the subtree was dropped as a loop. Only then is the model the same wherever
        // the blueprint is nested, and so safe to hand out again: a subtree that dropped a link to one of its
        // ancestors would, under a different path, have kept it.
        private (BlueprintModel Model, bool Acyclic) Blueprint(SnapshotBlueprint record)
        {
            if (_blueprints.TryGetValue(record.Id, out BlueprintModel? built))
            {
                return (built, true);
            }

            BlueprintModel model = new()
            {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                Value = record.Value,
                Yield = record.Yield,
                ProductionTime = record.ProductionTime,
                Category = CategoryOf(record.CategoryId)
            };

            foreach (QuantityLink link in record.Components)
            {
                model.Components.Add(Component(link.TargetId), link.Quantity);
            }

            _ancestors.Add(record.Id);
            bool acyclic = true;

            // Dropping a child that is already an ancestor is what keeps a cyclic row set loadable. The parts picker
            // never offers an ancestor as a child, so nothing can write one now, but a database filled in before that
            // could, and so can an import file. Recursing into it threw out of every screen that reads a blueprint,
            // which left the whole app unusable; the export screen shows such a blueprint so it can say so.
            foreach (QuantityLink link in record.Blueprints)
            {
                if (_ancestors.Contains(link.TargetId))
                {
                    acyclic = false;
                    continue;
                }

                (BlueprintModel child, bool childAcyclic) = Blueprint(_blueprintRecords[link.TargetId]);
                model.ChildBlueprints.Add(child, link.Quantity);
                acyclic &= childAcyclic;
            }

            // Popped rather than left set, so a blueprint nested by two branches of the same tree builds under both.
            _ancestors.Remove(record.Id);

            if (acyclic)
            {
                _blueprints[record.Id] = model;
            }

            return (model, acyclic);
        }

        private CategoryModel? CategoryOf(int? categoryId)
        {
            if (categoryId is not { } id)
            {
                return null;
            }

            if (_categories.TryGetValue(id, out CategoryModel? model))
            {
                return model;
            }

            model = ToCategoryModel(_categoryRecords[id]);
            _categories[id] = model;

            return model;
        }
    }
}
