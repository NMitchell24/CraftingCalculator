using System.Collections.ObjectModel;

namespace CraftingCalculator.Domain.Models;

public class BlueprintMap
{
    private readonly List<BlueprintQuantity> _internalList = [];
    public ReadOnlyCollection<BlueprintQuantity> BlueprintList => _internalList.AsReadOnly();

    /// <summary>
    /// Adds the Blueprint and quantity to the list if no entry has its id.
    /// If one does then it will just increase the quantity of the existing record
    /// by the quantity that is passed into this Add function.
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="quantity"></param>
    public void Add(BlueprintModel? blueprint, long quantity)
    {
        if (blueprint == null)
        {
            return;
        }

        BlueprintQuantity? existing = _internalList.Find(blueprintQuantity => blueprintQuantity.Blueprint.Id == blueprint.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _internalList.Add(new BlueprintQuantity(blueprint, quantity));
        }
    }

    /// <summary>
    /// Removes the entry with the blueprint's id from the list, if there is one.
    /// </summary>
    /// <param name="blueprint"></param>
    public void RemoveAll(BlueprintModel blueprint)
    {
        _internalList.RemoveAll(blueprintQuantity => blueprintQuantity.Blueprint.Id == blueprint.Id);
    }

    /// <summary>
    /// Removes exactly <paramref name="blueprintQuantity"/> from the list, leaving every other entry in place.
    /// </summary>
    public void Remove(BlueprintQuantity blueprintQuantity)
    {
        _internalList.Remove(blueprintQuantity);
    }

    /// <summary>
    /// Reset the Map and clear all BlueprintQuantity objects
    /// </summary>
    public void Reset()
    {
        _internalList.Clear();
    }
}
