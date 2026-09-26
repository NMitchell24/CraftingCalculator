using System.Collections.ObjectModel;

namespace CraftingCalculator.Domain.Models;

public class ComponentMap
{
    private readonly List<ComponentQuantity> _internalList = [];

    // The same entries as _internalList, by component id, so Add is a lookup rather than a scan. The list
    // stays the source of order, which is user-visible. Keyed by the id an entry was added with: a component's
    // id only changes when it is first saved (0 to its row id), and an unsaved component is never a part in a map.
    private readonly Dictionary<int, ComponentQuantity> _byId = [];

    public ReadOnlyCollection<ComponentQuantity> ComponentList => _internalList.AsReadOnly();

    /// <summary>
    /// Adds the component and quantity to the list if no entry has its id.
    /// If one does then it will just increase the quantity of the existing record
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

        if (_byId.TryGetValue(component.Id, out ComponentQuantity? existing))
        {
            existing.Quantity += quantity;
        }
        else
        {
            ComponentQuantity added = new(component, quantity);
            _byId.Add(component.Id, added);
            _internalList.Add(added);
        }
    }

    /// <summary>
    /// Removes the entry with the component's id from the list, if there is one.
    /// </summary>
    /// <param name="component"></param>
    public void RemoveAll(ComponentModel component)
    {
        if (_byId.Remove(component.Id, out ComponentQuantity? removed))
        {
            _internalList.Remove(removed);
        }
    }

    /// <summary>
    /// Reset the contents of the map and empty it.
    /// </summary>
    public void Reset()
    {
        _internalList.Clear();
        _byId.Clear();
    }
}
