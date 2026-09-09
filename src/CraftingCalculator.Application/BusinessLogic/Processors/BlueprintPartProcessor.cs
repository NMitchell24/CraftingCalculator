using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Edits the parts of a single blueprint - its own components plus the child blueprints nested inside it -
/// as one collection, so callers do not have to know which of <see cref="BlueprintModel.Components"/> or
/// <see cref="BlueprintModel.ChildBlueprints"/> a given part lives in.
/// </summary>
public static class BlueprintPartProcessor
{
    /// <summary>
    /// The blueprint's components and child blueprints as one list, components first and each group
    /// ordered by name.
    /// </summary>
    public static List<IBaseQuantityRecord> GetParts(BlueprintModel blueprint)
    {
        List<IBaseQuantityRecord> parts = [.. blueprint.Components.ComponentList];
        parts.AddRange(blueprint.ChildBlueprints.BlueprintList);

        return [.. parts.OrderBy(p => p.Type).ThenBy(p => p.Name)];
    }

    /// <summary>
    /// Adds the part to the blueprint, or raises the quantity of the matching part already on it by
    /// <paramref name="quantity"/>.
    /// </summary>
    public static void Add(BlueprintModel blueprint, IBaseDataRecord? part, long quantity)
    {
        switch (part)
        {
            case ComponentModel component:
                blueprint.Components.Add(component, quantity);
                break;
            case BlueprintModel child:
                blueprint.ChildBlueprints.Add(child, quantity);
                break;
        }
    }

    /// <summary>
    /// Sets the part's quantity on the blueprint. A quantity of 0 or less removes the part entirely.
    /// </summary>
    public static void SetQuantity(BlueprintModel blueprint, IBaseQuantityRecord part, long quantity)
    {
        if (quantity <= 0)
        {
            Remove(blueprint, part);
            return;
        }

        part.Quantity = quantity;
    }

    /// <summary>Removes the part from the blueprint.</summary>
    public static void Remove(BlueprintModel blueprint, IBaseQuantityRecord part)
    {
        // RemoveAll rather than a quantity of 0: it is what pushes the part onto the map's
        // RemovedComponents/RemovedBlueprints list, which is the only signal BlueprintDAO.SaveAsync has
        // that the underlying join row should be deleted.
        switch (part)
        {
            case ComponentQuantity componentQuantity:
                blueprint.Components.RemoveAll(componentQuantity.Component);
                break;
            case BlueprintQuantity blueprintQuantity:
                blueprint.ChildBlueprints.RemoveAll(blueprintQuantity.Blueprint);
                break;
        }
    }
}
