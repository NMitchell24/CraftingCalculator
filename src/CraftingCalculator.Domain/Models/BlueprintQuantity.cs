using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A class that represents a blueprint and quantity
/// </summary>
public class BlueprintQuantity : IBaseQuantityRecord
{
    public BlueprintModel Blueprint { get; set; }

    public long Quantity
    {
        get;
        set { field = Math.Abs(value); }
    }

    public string Name
    {
        get => Blueprint.Name ?? "";
        set
        {
            // Projected from Blueprint.Name - the setter exists only to satisfy IBaseQuantityRecord
            // and is deliberately inert, as on every projected member below.
        }
    }

    public DataType Type
    {
        get => Blueprint.Type;
        set
        {
            // Projected from Blueprint.Type - inert for the reason given on Name.
        }
    }

    public string Description
    {
        get => Blueprint.Description ?? "";
        set
        {
            // Projected from Blueprint.Description - inert for the reason given on Name.
        }
    }

    public IBaseDataRecord Record => Blueprint;

    public double TotalValue => Blueprint.Value * Quantity;

    public string DisplayName => Name + " x" + Quantity;

    public BlueprintQuantity(BlueprintModel blueprint, long quantity)
    {
        Blueprint = blueprint;
        Quantity = quantity;
    }
}
