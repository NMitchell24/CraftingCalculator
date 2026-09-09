using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Domain.Models;

public class BlueprintMap
{
    private readonly List<BlueprintQuantity> _internalList = new List<BlueprintQuantity>();
    public ReadOnlyCollection<BlueprintQuantity> BlueprintList => _internalList.AsReadOnly();
    public List<BlueprintQuantity> RemovedBlueprints { get; } = new List<BlueprintQuantity>();

    public BlueprintMap(BlueprintMap map, bool cloneForSave)
    {
        _internalList = new List<BlueprintQuantity>();
        foreach (BlueprintQuantity blueprintQuantity in map.BlueprintList)
        {
            if (cloneForSave)
            {
                _internalList.Add(blueprintQuantity.CloneForSave());
            }
            else
            {
                _internalList.Add(blueprintQuantity.Clone());
            }
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
        if (blueprint != null)
        {
            Add(blueprint, quantity, 0);
        }
    }

    /// <summary>
    /// Overload to preserve the data Identifier from the database.
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="quantity"></param>
    /// <param name="id"></param>
    public void Add(BlueprintModel blueprint, long quantity, int id)
    {
        if (_internalList.Any(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name))
        {
            BlueprintQuantity? existing = _internalList.Find(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
        }
        else
        {
            _internalList.Add(new BlueprintQuantity(blueprint, quantity, id));
        }
    }

    /// <summary>
    /// Decrement a BlueprintQuantity by the provided amount.  
    /// If the current Quantity - the provided quantity would be less than or equal to 0
    /// Then the blueprint will be removed.
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="quantity"></param>
    public void Remove(BlueprintModel blueprint, long quantity)
    {
        if (_internalList.Any(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name && blueprintQuantity.Quantity - quantity > 0))
        {
            Add(blueprint, -quantity);
        }
        else
        {
            RemoveAll(blueprint);
        }

    }

    /// <summary>
    /// Remove a BlueprintQuantity from the list entirely if it exists.
    /// </summary>
    /// <param name="blueprint"></param>
    public void RemoveAll(BlueprintModel blueprint)
    {
        if (_internalList.Any(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name))
        {
            BlueprintQuantity? existing = _internalList.Find(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name);
            if (existing != null)
            {
                RemovedBlueprints.AddRange(_internalList.FindAll(blueprintQuantity => blueprintQuantity.Blueprint.Name == blueprint.Name));
                _internalList.Remove(existing);
            }
        }
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
        return new BlueprintMap(this, false);
    }

    public BlueprintMap CloneForSave()
    {
        return new BlueprintMap(this, true);
    }
}
