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
    public BlueprintFilter? Filter { get; set; }
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

            if (ChildBlueprints != null)
            {
                foreach (BlueprintQuantity blueprint in ChildBlueprints.BlueprintList)
                {
                    sb.AppendLine(blueprint.Name + " x" + blueprint.Quantity);
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
            return DataType.Blueprint;
        }
        //Don't allow this to be changed as it should remain static.
        set { }
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
            Filter = this.Filter,
            Value = this.Value,
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
