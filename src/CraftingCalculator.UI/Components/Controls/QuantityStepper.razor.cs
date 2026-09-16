using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>A quantity field between buttons that step it down or up by one and by ten.</summary>
public partial class QuantityStepper : ComponentBase
{
    /// <summary>The quantity shown in the field.</summary>
    [Parameter, EditorRequired] public long Quantity { get; set; }

    /// <summary>Raised with -10, -1, 1 or 10 when a step button is tapped.</summary>
    [Parameter] public EventCallback<long> OnStep { get; set; }

    /// <summary>Raised when a quantity is typed into the field.</summary>
    [Parameter] public EventCallback<long> OnQuantityChanged { get; set; }
}
