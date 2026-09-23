using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models;

/// <summary>
/// Represents an individual UI Model for the Blueprints
/// </summary>
public class BlueprintModel : ICategorizedRecord
{
    public ComponentMap Components { get; } = new();
    public BlueprintMap ChildBlueprints { get; } = new();
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
}
