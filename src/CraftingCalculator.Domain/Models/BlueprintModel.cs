using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents an individual UI Model for the Blueprints
/// </summary>
public class BlueprintModel : IBaseDataRecord
{
    public ComponentMap Components { get; private set; }
    public BlueprintMap ChildBlueprints { get; private set; }
    public string? Name { get; set; }
    public int Id { get; set; }
    public string? Description { get; set; }
    public CategoryModel? Category { get; set; }
    public double Value { get; set; }

    /// <summary>
    /// How many items one craft of this blueprint produces. Always at least 1.
    /// </summary>
    public long Yield
    {
        get;
        //Guards the divide in BlueprintProcessor.CraftsFor. A yield of 0 or less has no meaning and
        //would either divide by zero or produce a negative craft count, so it is pinned to the
        //1-per-craft default rather than rejected - the editor's Min="1" is the user-facing validation.
        set => field = value < 1 ? 1 : value;
    } = 1;

    /// <summary>
    /// How long one craft of this blueprint takes. <see cref="TimeSpan.Zero"/> means instant.
    /// </summary>
    public TimeSpan ProductionTime
    {
        get;
        //Mirrors the Yield clamp above. A negative duration would subtract from the batch total and
        //could drive it below zero, so it is pinned to Zero rather than rejected - the editor's Min="0"
        //on each field is the user-facing validation.
        set => field = value < TimeSpan.Zero ? TimeSpan.Zero : value;
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
            _ = value;
        }
    }

    /// <summary>
    /// Default constructor.  ensures maps are initialized.
    /// </summary>
    public BlueprintModel()
    {
        Components = new ComponentMap();
        ChildBlueprints = new BlueprintMap();
    }

    public bool IsSelected { get; set; }

    public IBaseDataRecord Clone()
    {
        BlueprintModel clone = new()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Category = Category,
            Value = Value,
            Yield = Yield,
            ProductionTime = ProductionTime,
            Components = Components.Clone(),
            ChildBlueprints = ChildBlueprints.Clone()
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        BlueprintModel ret = (BlueprintModel)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;
        ret.Components = Components.CloneForSave();
        ret.ChildBlueprints = ChildBlueprints.CloneForSave();

        return ret;
    }
}
