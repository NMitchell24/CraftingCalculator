using CraftingCalculator.Domain.Enums;
using System;

namespace CraftingCalculator.Domain.Models;

public class Component : IBaseDataRecord
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Cost { get; set; }
    public string Tooltip
    {
        get
        {
            string ret = Description ?? "";
            if (Cost > 0)
            {
                ret = ret +
                    Environment.NewLine + Environment.NewLine +
                    "Cost Per Item: " + string.Format("{0:C2}", Cost);
            }
            return ret;
        }
        set { }
    }
    public DataType Type
    {
        get
        {
            return DataType.Component;
        }
        //Don't allow this to be changed as it should remain static.
        set { }
    }

    public IBaseDataRecord Clone()
    {
        Component clone = new Component()
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Cost = this.Cost
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        Component ret = (Component)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;

        return ret;
    }
}
