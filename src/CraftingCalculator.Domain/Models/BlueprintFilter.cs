using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Simple Class to define the UI Model for BlueprintFilters
/// </summary>
public class BlueprintFilter : IBaseDataRecord
{
    public const string ALL = "All";
    public string? Name { get; set; }
    public int Id { get; set; }
    public string? Description { get; set; }
    public string Tooltip { get => Description ?? ""; set { } }
    public DataType Type
    {
        get
        {
            return DataType.BlueprintFilter;
        }
        //Don't allow this to be changed as it should remain static
        set { }
    }

    public IBaseDataRecord Clone()
    {
        BlueprintFilter clone = new BlueprintFilter()
        {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description
        };

        return clone;
    }

    public IBaseDataRecord CopyForSave()
    {
        BlueprintFilter ret = (BlueprintFilter)Clone();
        ret.Name += " - Copy";
        ret.Id = 0;

        return ret;
    }
}
