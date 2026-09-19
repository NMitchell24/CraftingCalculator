namespace CraftingCalculator.Domain.Models;

/// <summary>
/// The settings that belong to one dataset rather than to the app: they follow the dataset through a
/// switch, a copy, and an export and import. Each parameter's default is the setting a new dataset starts with.
/// </summary>
/// <param name="UseYield">
/// Whether a blueprint's yield per craft counts. When false every blueprint makes one item per craft,
/// whatever its stored yield, so nothing is ever overproduced.
/// </param>
/// <param name="UseCosts">
/// Whether a component's cost counts. When false every component costs 0, whatever its stored cost, so a
/// batch costs nothing and its profit is its value.
/// </param>
/// <param name="UseValues">
/// Whether a blueprint's value counts. When false every blueprint is worth 0, whatever its stored value, so a
/// batch is worth nothing and its profit is the negative of its cost.
/// </param>
/// <param name="UseCraftTime">
/// Whether production time is shown. When false no production time is shown anywhere, whatever the stored times.
/// </param>
/// <param name="CurrencyLabel">
/// What the dataset's money is called, written after every amount ("200 Gold"). Null means the device's own
/// currency ("$200.00").
/// </param>
public sealed record Datasettings(bool UseYield = true,
    bool UseCosts = true,
    bool UseValues = true,
    bool UseCraftTime = true,
    string? CurrencyLabel = null)
{
    /// <summary>The settings a new dataset starts with, and that a record written before a setting existed implies.</summary>
    public static Datasettings Default { get; } = new();
}
