using CraftingCalculator.Domain.Enums;
using System;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A class that represents a recipe and quantity
/// </summary>
public class RecipeQuantity : IBaseQuantityRecord
{
    public int Id { get; set; }
    public Recipe Recipe { get; set; }
    private long _quantity;
    public long Quantity
    {
        get => _quantity;
        set
        {
            _quantity = Math.Abs(value);
        }
    }
    public string Name { get => Recipe.Name ?? ""; set { } }
    public string FilterType { get => Recipe.Filter?.Name ?? ""; set { } }
    public DataType Type { get => Recipe.Type; set { } }
    public string Description { get => Recipe.Description ?? ""; set { } }
    public double TotalValue { get => Recipe.Value * Quantity; set { } }
    public string Tooltip
    {
        get
        {
            return Recipe.Tooltip;
        }

        set { }
    }
    public bool IsSelected { get; set; }

    public RecipeQuantity(Recipe recipe, long quantity, int id)
    {
        Recipe = recipe;
        Quantity = quantity;
        Id = id;
    }

    /// <summary>
    /// Clone this quantity object
    /// </summary>
    /// <returns></returns>
    public RecipeQuantity Clone()
    {
        return new RecipeQuantity(Recipe, Quantity, Id);
    }

    /// <summary>
    /// Clones for saving clears ID so that it can be saved as a new record.
    /// </summary>
    /// <returns></returns>
    public RecipeQuantity CloneForSave()
    {
        RecipeQuantity ret = this.Clone();
        ret.Id = 0;
        return ret;
    }
}
