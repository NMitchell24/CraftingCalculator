using CraftingCalculator.Domain.Enums;
using System;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A class that represents a blueprint and quantity
/// </summary>
public class BlueprintQuantity : IBaseQuantityRecord
{
    public int Id { get; set; }
    public BlueprintModel Blueprint { get; set; }
    private long _quantity;
    public long Quantity
    {
        get => _quantity;
        set
        {
            _quantity = Math.Abs(value);
        }
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

    public double TotalValue => Blueprint.Value * Quantity;

    public bool IsSelected { get; set; }

    public string DisplayName => Name + " x" + Quantity;

    public BlueprintQuantity(BlueprintModel blueprint, long quantity, int id)
    {
        Blueprint = blueprint;
        Quantity = quantity;
        Id = id;
    }

    /// <summary>
    /// Clone this quantity object
    /// </summary>
    /// <returns></returns>
    public BlueprintQuantity Clone()
    {
        return new BlueprintQuantity(Blueprint, Quantity, Id);
    }

    /// <summary>
    /// Clones for saving clears ID so that it can be saved as a new record.
    /// </summary>
    /// <returns></returns>
    public BlueprintQuantity CloneForSave()
    {
        BlueprintQuantity ret = this.Clone();
        ret.Id = 0;
        return ret;
    }
}
