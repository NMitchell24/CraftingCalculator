using System.Globalization;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Renders an amount of money in the currency a dataset uses, and turns what a player typed into a currency label.
/// </summary>
public static class CurrencyProcessor
{
    /// <summary>The most characters a currency label can have.</summary>
    public const int MaxLabelLength = 12;

    /// <summary>
    /// Formats <paramref name="amount"/> in the device's currency with two decimals ("$1,234.50") when
    /// <paramref name="settings"/> has no currency label. Otherwise formats it as the number followed by the label,
    /// with only as many decimals as it needs, up to two ("1,234 Gold", "12.5 Gold").
    /// </summary>
    public static string Format(double amount, Datasettings settings)
    {
        double rounded = Round(amount);

        return settings.CurrencyLabel is { } label
            ? string.Format(CultureInfo.CurrentCulture, "{0:#,0.##} {1}", rounded, label)
            : string.Format(CultureInfo.CurrentCulture, "{0:C2}", rounded);
    }

    /// <summary>
    /// <paramref name="amount"/> as <see cref="Format"/> shows it: to the nearest hundredth, so an amount that only
    /// misses zero by rounding error is zero, and neither positive nor negative.
    /// </summary>
    public static double Round(double amount) =>
        // A sum that should be zero can land just below it (0.3 - 0.1 * 3). Rounding it gives a negative zero, which
        // both formats print as "-0"; adding a positive zero makes it positive.
        Math.Round(amount, 2, MidpointRounding.AwayFromZero) + 0.0;

    /// <summary>
    /// The currency label <paramref name="text"/> stands for: trimmed, or null (the device's currency) when it is
    /// empty or only whitespace.
    /// </summary>
    public static string? ToLabel(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
