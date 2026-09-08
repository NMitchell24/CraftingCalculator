using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents a component and quantity.
/// </summary>
public class ComponentQuantity : IBaseQuantityRecord
{
    public int Id { get; set; }
    public Component Component { get; set; }
    public long Quantity { get; set; }

    public string Name
    {
        get => Component.Name ?? "";
        set
        {
            // Projected from Component.Name - the setter exists only to satisfy IBaseQuantityRecord
            // and is deliberately inert, as on every projected member below.
        }
    }

    public string Description
    {
        get => Component.Description ?? "";
        set
        {
            // Projected from Component.Description - inert for the reason given on Name.
        }
    }

    public string Tooltip
    {
        get => Component.Tooltip;
        set
        {
            // Projected from Component.Tooltip - inert for the reason given on Name.
        }
    }

    public DataType Type
    {
        get => DataType.Component;
        set
        {
            // Fixed for this type - inert for the reason given on Name.
        }
    }

    public double TotalCost => Component.Cost * Quantity;

    public string DisplayName => Name + " x" + Quantity;

    public ComponentQuantity(Component component, long quantity, int id)
    {
        Component = component;
        Quantity = quantity;
        Id = id;
    }

    public ComponentQuantity Clone()
    {
        return new ComponentQuantity(Component, Quantity, Id);
    }

    public ComponentQuantity CloneForSave()
    {
        ComponentQuantity ret = this.Clone();
        ret.Id = 0;
        return ret;
    }
}
