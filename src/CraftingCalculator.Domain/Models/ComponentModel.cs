using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

public class ComponentModel : IBaseDataRecord
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double Cost { get; set; }

    /// <summary>
    /// How long one of this component takes to gather or produce. <see cref="TimeSpan.Zero"/> means
    /// instant, which is the case for a component simply taken from stock.
    /// </summary>
    public TimeSpan ProductionTime
    {
        get;
        //Clamped for the reason given on Blueprint.ProductionTime.
        set => field = value < TimeSpan.Zero ? TimeSpan.Zero : value;
    }

    public DataType Type
    {
        get
        {
            return DataType.Component;
        }
        set
        {
            //Don't allow this to be changed as it should remain static.
        }
    }

    public IBaseDataRecord Clone()
    {
        ComponentModel clone = new()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Cost = Cost,
            ProductionTime = ProductionTime
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        ComponentModel ret = (ComponentModel)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;

        return ret;
    }
}
