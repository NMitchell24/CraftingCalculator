using CraftingCalculator.Domain.Enums;
using System;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A class that represents a blueprint and quantity
/// </summary>
public class BlueprintQuantity : IBaseQuantityRecord
{
    public int Id { get; set; }
    public Blueprint Blueprint { get; set; }
    private long _quantity;
    public long Quantity
    {
        get => _quantity;
        set
        {
            _quantity = Math.Abs(value);
        }
    }
    public string Name { get => Blueprint.Name ?? ""; set { } }
    public string CategoryName { get => Blueprint.Category?.Name ?? ""; set { } }
    public DataType Type { get => Blueprint.Type; set { } }
    public string Description { get => Blueprint.Description ?? ""; set { } }
    public double TotalValue { get => Blueprint.Value * Quantity; set { } }
    public string Tooltip
    {
        get
        {
            return Blueprint.Tooltip;
        }

        set { }
    }
    public bool IsSelected { get; set; }

    public BlueprintQuantity(Blueprint blueprint, long quantity, int id)
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
