using CraftingCalculator.Domain.Enums;
using System.Text;
using System;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents an individual UI Model for the Blueprints
/// </summary>
public class Blueprint : IBaseDataRecord
{
    public ComponentMap Components { get; set; }
    public BlueprintMap ChildBlueprints { get; set; }
    public string? Name { get; set; }
    public int Id { get; set; }
    public string? Description { get; set; }
    public Category? Category { get; set; }
    public double Value { get; set; }

    private long _yield = 1;

    /// <summary>
    /// How many items one craft of this blueprint produces. Always at least 1.
    /// </summary>
    public long Yield
    {
        get => _yield;
        //Guards the divide in BlueprintProcessor.CraftsFor. A yield of 0 or less has no meaning and
        //would either divide by zero or produce a negative craft count, so it is pinned to the
        //1-per-craft default rather than rejected - the editor's Min="1" is the user-facing validation.
        set => _yield = value < 1 ? 1 : value;
    }

    public string Tooltip
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Name);
            sb.AppendLine(Category?.Name);
            if (Value > 0)
            {
                sb.AppendLine("Value per Item: " + string.Format("{0:C2}", Value));
            }
            if (Yield > 1)
            {
                sb.AppendLine("Yield per Craft: " + Yield);
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

            if (ChildBlueprints != null)
            {
                foreach (BlueprintQuantity blueprint in ChildBlueprints.BlueprintList)
                {
                    sb.AppendLine(blueprint.Name + " x" + blueprint.Quantity);
                }
            }

            return sb.ToString();
        }
        set
        {
            // Computed from Name/Description/Components - the setter exists only to satisfy
            // IBaseDataRecord and is deliberately inert, as on every other model's Tooltip.
        }
    }

    public DataType Type
    {
        get
        {
            return DataType.Blueprint;
        }

        set
        {
            //Don't allow this to be changed as it should remain static.
        }
    }

    /// <summary>
    /// Default constructor.  ensures maps are initialized.
    /// </summary>
    public Blueprint()
    {
        this.Components = new ComponentMap();
        this.ChildBlueprints = new BlueprintMap();
    }

    public bool IsSelected { get; set; }

    public IBaseDataRecord Clone()
    {
        Blueprint clone = new Blueprint()
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Category = this.Category,
            Value = this.Value,
            Yield = this.Yield,
            Components = this.Components.Clone(),
            ChildBlueprints = this.ChildBlueprints.Clone()
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        Blueprint ret = (Blueprint)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;
        ret.Components = this.Components.CloneForSave();
        ret.ChildBlueprints = this.ChildBlueprints.CloneForSave();

        return ret;
    }
}
