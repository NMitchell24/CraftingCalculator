using System.Globalization;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Renders a production time as the compact form the Crafting Summary and Crafting Steps display,
/// widening the format only as far as the value requires: "Instant", "5.5s", "4:30", "2:04:30", or
/// "3 Days 02:04:30".
/// </summary>
public static class DurationProcessor
{
    /// <summary>
    /// Formats <paramref name="duration"/> for display. Zero and any negative value read as "Instant".
    /// </summary>
    public static string Format(TimeSpan duration) => duration switch
    {
        { Ticks: <= 0 } => "Instant",
        //Tenths survive only below a minute. The MM:SS and wider forms have no slot for a fraction, and
        //a batch measured in hours gains nothing from one. "0.#" drops a trailing ".0" so a whole
        //number of seconds reads "45s" rather than "45.0s".
        { TotalMinutes: < 1 } => duration.TotalSeconds.ToString("0.#", CultureInfo.CurrentCulture) + "s",
        { TotalHours: < 1 } => $"{duration.Minutes}:{duration.Seconds:00}",
        //TotalHours rather than Hours: past a day Hours restarts at 0, and this arm is the sub-day case.
        { TotalDays: < 1 } => $"{(int)duration.TotalHours}:{duration.Minutes:00}:{duration.Seconds:00}",
        _ => $"{duration.Days} {(duration.Days == 1 ? "Day" : "Days")} " +
             $"{duration.Hours:00}:{duration.Minutes:00}:{duration.Seconds:00}"
    };
}
