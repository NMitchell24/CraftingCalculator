using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Domain.Models;

public class BlueprintMap
{
    private List<BlueprintQuantity> _internalList = new List<BlueprintQuantity>();
    public ReadOnlyCollection<BlueprintQuantity> BlueprintList
    {
        get => _internalList.AsReadOnly();
        private set { }
    }
    public List<BlueprintQuantity> RemovedBlueprints = new List<BlueprintQuantity>();

    public BlueprintMap(BlueprintMap map, bool cloneForSave)
    {
        _internalList = new List<BlueprintQuantity>();
        foreach (BlueprintQuantity r in map.BlueprintList)
        {
            if (cloneForSave)
            {
                _internalList.Add(r.CloneForSave());
            }
            else
            {
                _internalList.Add(r.Clone());
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
    public void Add(Blueprint? blueprint, long quantity)
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
    public void Add(Blueprint blueprint, long quantity, int id)
    {
        if (_internalList.Any(i => i.Blueprint.Name == blueprint.Name))
        {
            BlueprintQuantity? rq = _internalList.Find(i => i.Blueprint.Name == blueprint.Name);
            if (rq != null)
            {
                rq.Quantity += quantity;
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
    public void Remove(Blueprint blueprint, long quantity)
    {
        if (_internalList.Any(i => i.Blueprint.Name == blueprint.Name && i.Quantity - quantity > 0))
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
    public void RemoveAll(Blueprint blueprint)
    {
        if (_internalList.Any(i => i.Blueprint.Name == blueprint.Name))
        {
            BlueprintQuantity? rq = _internalList.Find(i => i.Blueprint.Name == blueprint.Name);
            if (rq != null)
            {
                RemovedBlueprints.AddRange(_internalList.FindAll(i => i.Blueprint.Name == blueprint.Name));
                _internalList.Remove(rq);
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
