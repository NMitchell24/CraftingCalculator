namespace CraftingCalculator.Domain.Models;

/// <summary>
/// The settings that belong to one dataset rather than to the app: they follow the dataset through a
/// switch, a copy, and an export and import. Each parameter's default is the setting a new dataset starts with.
/// </summary>
/// <param name="UseYield">
/// Whether a blueprint's yield per craft counts. When false every blueprint makes one item per craft,
/// whatever its stored yield, so nothing is ever overproduced.
/// </param>
public sealed record Datasettings(bool UseYield = true)
{
    /// <summary>The settings a new dataset starts with, and that a record written before a setting existed implies.</summary>
    public static Datasettings Default { get; } = new();
}
