using System.Collections.ObjectModel;

namespace CraftingCalculator.Domain.Models;

public class ComponentMap
{
    private readonly List<ComponentQuantity> _internalList = [];
    public ReadOnlyCollection<ComponentQuantity> ComponentList => _internalList.AsReadOnly();

    private ComponentMap(ComponentMap map)
    {
        _internalList = [];
        foreach (ComponentQuantity componentQuantity in map.ComponentList)
        {
            // Ensures we don't retain references to the original ComponentQuantity object and cause those
            // objects to get mutated elsewhere.
            _internalList.Add(componentQuantity.Clone());
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
    public void Add(ComponentModel? component, long quantity)
    {
        if (component == null)
        {
            return;
        }

        ComponentQuantity? existing = _internalList.Find(componentQuantity => componentQuantity.Name == component.Name);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _internalList.Add(new ComponentQuantity(component, quantity));
        }
    }

    /// <summary>
    /// Decrement a ComponentQuantity by the provided amount.
    /// If the current Quantity - the provided quantity would be less than or equal to 0
    /// Then the Component will be removed.
    /// </summary>
    /// <param name="component"></param>
    /// <param name="quantity"></param>
    public void Remove(ComponentModel component, long quantity)
    {
        if (_internalList.Any(componentQuantity => componentQuantity.Name == component.Name && componentQuantity.Quantity - quantity > 0))
        {
            Add(component, -quantity);
        }
        else
        {
            RemoveAll(component);
        }

    }

    /// <summary>
    /// Remove a Component from the list entirely if it exists.
    /// </summary>
    /// <param name="component"></param>
    public void RemoveAll(ComponentModel component)
    {
        _internalList.RemoveAll(componentQuantity => componentQuantity.Name == component.Name);
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
        return new ComponentMap(this);
    }
}
