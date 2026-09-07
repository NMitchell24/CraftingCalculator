using CraftingCalculator.Domain.Enums;
using System.Text;
using System;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents an individual UI Model for the Recipes
/// </summary>
public class Recipe : IBaseDataRecord
{
    public ComponentMap Components { get; set; }
    public RecipeMap ChildRecipes { get; set; }
    public string? Name { get; set; }
    public int Id { get; set; }
    public string? Description { get; set; }
    public RecipeFilter? Filter { get; set; }
    public double Value { get; set; }

    public string Tooltip
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Name);
            sb.AppendLine(Filter?.Name);
            if (Value > 0)
            {
                sb.AppendLine("Value per Item: " + string.Format("{0:C2}", Value));
            }
            sb.Append(Environment.NewLine);
            if (Description != null && Description.Length > 0)
            {
                sb.AppendLine("Description:");
                sb.AppendLine(Description);
                sb.Append(Environment.NewLine);
            }
            sb.AppendLine("Components:");

            foreach (ComponentQuantity component in Components.ComponentList)
            {
                sb.AppendLine(component.Name + " x" + component.Quantity);
            }

            if (ChildRecipes != null)
            {
                foreach (RecipeQuantity recipe in ChildRecipes.RecipeList)
                {
                    sb.AppendLine(recipe.Name + " x" + recipe.Quantity);
                }
            }

            return sb.ToString();
        }
        //Computed from Name/Description/Components - the setter exists only to satisfy
        //IBaseDataRecord and is deliberately inert, as on every other model's Tooltip.
        set { }
    }

    public DataType Type
    {
        get
        {
            return DataType.Recipe;
        }
        //Don't allow this to be changed as it should remain static.
        set { }
    }

    /// <summary>
    /// Default constructor.  ensures maps are initialized.
    /// </summary>
    public Recipe()
    {
        this.Components = new ComponentMap();
        this.ChildRecipes = new RecipeMap();
    }

    public bool IsSelected { get; set; }

    public IBaseDataRecord Clone()
    {
        Recipe clone = new Recipe()
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Filter = this.Filter,
            Value = this.Value,
            Components = this.Components.Clone(),
            ChildRecipes = this.ChildRecipes.Clone()
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        Recipe ret = (Recipe)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;
        ret.Components = this.Components.CloneForSave();
        ret.ChildRecipes = this.ChildRecipes.CloneForSave();

        return ret;
    }
}
