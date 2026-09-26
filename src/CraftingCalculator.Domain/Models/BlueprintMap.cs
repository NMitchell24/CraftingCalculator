using System.Collections.ObjectModel;

namespace CraftingCalculator.Domain.Models;

public class BlueprintMap
{
    private readonly List<BlueprintQuantity> _internalList = [];

    // The same entries as _internalList, by blueprint id, so Add is a lookup rather than a scan. The list
    // stays the source of order, which is user-visible. Keyed by the id an entry was added with:
    // BlueprintQuantity.Blueprint can be reassigned (CraftState.ReloadBlueprintsAsync does), but only ever to a
    // newer copy of the same blueprint, so the id and therefore the key never change.
    private readonly Dictionary<int, BlueprintQuantity> _byId = [];

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

        if (_byId.TryGetValue(blueprint.Id, out BlueprintQuantity? existing))
        {
            existing.Quantity += quantity;
        }
        else
        {
            BlueprintQuantity added = new(blueprint, quantity);
            _byId.Add(blueprint.Id, added);
            _internalList.Add(added);
        }
    }

    /// <summary>
    /// Removes the entry with the blueprint's id from the list, if there is one.
    /// </summary>
    /// <param name="blueprint"></param>
    public void RemoveAll(BlueprintModel blueprint)
    {
        if (_byId.Remove(blueprint.Id, out BlueprintQuantity? removed))
        {
            _internalList.Remove(removed);
        }
    }

    /// <summary>
    /// Removes exactly <paramref name="blueprintQuantity"/> from the list, leaving every other entry in place.
    /// </summary>
    public void Remove(BlueprintQuantity blueprintQuantity)
    {
        // Only an entry that was in the list owns its id's index slot; one that wasn't must leave the slot alone.
        if (_internalList.Remove(blueprintQuantity))
        {
            _byId.Remove(blueprintQuantity.Blueprint.Id);
        }
    }

    /// <summary>
    /// Reset the Map and clear all BlueprintQuantity objects
    /// </summary>
    public void Reset()
    {
        _internalList.Clear();
        _byId.Clear();
    }
}
