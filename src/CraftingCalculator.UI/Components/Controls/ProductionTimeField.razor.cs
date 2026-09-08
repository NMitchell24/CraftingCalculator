using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// Edits a production time as hours, minutes and seconds-with-tenths. Shared by the Blueprint and
/// Component editors, which both bind a <see cref="TimeSpan"/> straight to <see cref="Value"/>.
/// </summary>
public partial class ProductionTimeField : ComponentBase
{
    [Parameter] public TimeSpan Value { get; set; }

    [Parameter] public EventCallback<TimeSpan> ValueChanged { get; set; }

    [Parameter] public string Label { get; set; } = "Production time";

    private int _hours;
    private int _minutes;
    private double _seconds;

    protected override void OnParametersSet()
    {
        // Only re-split when the incoming value is something other than what the three fields already
        // describe. Splitting on every render would overwrite a part-typed entry as the user types,
        // since each field notifies on change and comes straight back through Value.
        if (Compose() != Value)
        {
            // TotalHours, not Hours: hours are unbounded here, so a value past a day has to stay in the
            // hours field rather than silently losing its days.
            _hours = (int)Value.TotalHours;
            _minutes = Value.Minutes;
            _seconds = Value.Seconds + (Value.Milliseconds / 1000d);
        }
    }

    private TimeSpan Compose() =>
        TimeSpan.FromHours(_hours) + TimeSpan.FromMinutes(_minutes) + TimeSpan.FromSeconds(_seconds);

    private Task NotifyChangedAsync() => ValueChanged.InvokeAsync(Compose());
}
