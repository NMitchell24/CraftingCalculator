using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace CraftingCalculator.Domain.Models;

public class ComponentMap
{
    private List<ComponentQuantity> _internalList = new List<ComponentQuantity>();
    public ReadOnlyCollection<ComponentQuantity> ComponentList
    {
        get => _internalList.AsReadOnly();
        private set { }
    }
    public List<ComponentQuantity> RemovedComponents = new List<ComponentQuantity>();

    public ComponentMap(ComponentMap map, bool cloneForSave)
    {
        _internalList = new List<ComponentQuantity>();
        foreach (ComponentQuantity i in map.ComponentList)
        {
            // Insures we don't retain references to the original ComponentQuantity object and cause those 
            // objects to get mutated elsewhere.
            if (cloneForSave)
            {
                _internalList.Add(i.CloneForSave());
            }
            else
            {
                _internalList.Add(i.Clone());
            }
        }
    }

    public ComponentMap()
    {
        //nothing
    }
    /// <summary>
    /// Adds the component and quantity to the list if it does not exist.
    /// If it does exist then it will just increase the quantity of the existing record
    /// by the quantity that is passed into this Add function.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="quantity"></param>
    public void Add(Component? component, long quantity)
    {
        if (component != null)
        {
            Add(component, quantity, 0);
        }
    }

    public void Add(Component component, long quantity, int id)
    {
        if (_internalList.Any(i => i.Name == component.Name))
        {
            ComponentQuantity? iq = _internalList.Find(i => i.Name == component.Name);
            if (iq != null)
            {
                iq.Quantity += quantity;
            }
        }
        else
        {
            _internalList.Add(new ComponentQuantity(component, quantity, id));
        }
    }

    /// <summary>
    /// Decrement an ComponentQuantity by the provided amount.  
    /// If the current Quantity - the provided quantity would be less than or equal to 0
    /// Then the Component will be removed.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="quantity"></param>
    public void Remove(Component component, long quantity)
    {
        if (_internalList.Any(i => i.Name == component.Name && i.Quantity - quantity > 0))
        {
            Add(component, -quantity);
        }
        else
        {
            RemoveAll(component);
        }

    }

    /// <summary>
    /// Remove an ComponentType from the list entirely if it exists.
    /// </summary>
    /// <param name="component"></param>
    public void RemoveAll(Component component)
    {
        if (_internalList.Any(i => i.Name == component.Name))
        {
            ComponentQuantity? iq = _internalList.Find(i => i.Name == component.Name);
            if (iq != null)
            {
                RemovedComponents.Add(iq);
                _internalList.Remove(iq);
            }
        }
    }

    /// <summary>
    /// Reset the contents of the map and empty it.
    /// </summary>
    public void Reset()
    {
        _internalList.Clear();
    }

    /// <summary>
    /// Return a clone of this map.
    /// </summary>
    /// <returns></returns>
    public ComponentMap Clone()
    {
        return new ComponentMap(this, false);
    }

    /// <summary>
    /// Return a clone of this map with the ID values on the internal ComponentQuantity objects cleared.
    /// </summary>
    /// <returns></returns>
    public ComponentMap CloneForSave()
    {
        return new ComponentMap(this, true);
    }
}
