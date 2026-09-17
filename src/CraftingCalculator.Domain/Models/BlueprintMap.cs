using System.Collections.ObjectModel;

namespace CraftingCalculator.Domain.Models;

public class BlueprintMap
{
    private readonly List<BlueprintQuantity> _internalList = [];
    public ReadOnlyCollection<BlueprintQuantity> BlueprintList => _internalList.AsReadOnly();

    private BlueprintMap(BlueprintMap map)
    {
        _internalList = [];
        foreach (BlueprintQuantity blueprintQuantity in map.BlueprintList)
        {
            _internalList.Add(blueprintQuantity.Clone());
        }
    }

    public BlueprintMap()
    {
        //nothing
    }

    /// <summary>
    /// Adds the Blueprint and quantity to the list if it does not exist.
    /// If it does exist then it will just increase the quantity of the existing record
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

        BlueprintQuantity? existing = _internalList.Find(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name);
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
    /// Remove a BlueprintQuantity from the list entirely if it exists.
    /// </summary>
    /// <param name="blueprint"></param>
    public void RemoveAll(BlueprintModel blueprint)
    {
        _internalList.RemoveAll(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name);
    }

    /// <summary>
    /// Reset the Map and clear all BlueprintQuantity objects
    /// </summary>
    public void Reset()
    {
        _internalList.Clear();
    }

    public BlueprintMap Clone()
    {
        return new BlueprintMap(this);
    }
}
