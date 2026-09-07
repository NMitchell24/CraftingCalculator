using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents an component and quantity.
/// </summary>
public class ComponentQuantity : IBaseQuantityRecord
{
    public int Id { get; set; }
    public Component Component { get; set; }
    public long Quantity { get; set; }
    public string Name { get => Component.Name ?? ""; set { } }
    public string Description { get => Component.Description ?? ""; set { } }
    public string Tooltip { get => Component.Tooltip; set { } }
    public DataType Type { get => DataType.Component; set { } }
    public double TotalCost { get => Component.Cost * Quantity; set { } }

    public string DisplayName
    {
        get => Name + " x" + Quantity;
        private set { }
    }

    public ComponentQuantity(Component ing, long quantity, int id)
    {
        Component = ing;
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
