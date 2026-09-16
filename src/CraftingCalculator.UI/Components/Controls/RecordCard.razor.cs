using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// One record in a list, as a card of up to three stacked zones: the record's identity with its Info button, an
/// optional stepper zone, and an optional zone of actions. Placed inside the owner's MudListItem.
/// </summary>
public partial class RecordCard : ComponentBase
{
    /// <summary>The record's name, shown as the card's first line.</summary>
    [Parameter, EditorRequired] public string Name { get; set; } = "";

    /// <summary>Caption under the name: "Blueprint", "Component", "Category" or "Favorite".</summary>
    [Parameter, EditorRequired] public string Kind { get; set; } = "";

    /// <summary>The record whose category chip trails the kind; null for a row with no category (a favorite).</summary>
    [Parameter] public IBaseDataRecord? Record { get; set; }

    /// <summary>Extra identity lines, one per entry.</summary>
    [Parameter] public IReadOnlyList<string> Details { get; set; } = [];

    /// <summary>Invoked by the Info button.</summary>
    [Parameter, EditorRequired] public EventCallback OnInfo { get; set; }

    /// <summary>The stepper zone: a QuantityStepper, or a picker's Add button. Null when the row has no quantity.</summary>
    [Parameter] public RenderFragment? Stepper { get; set; }

    /// <summary>The actions zone, right-aligned. Null when the row has no actions.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    private string InfoLabel => $"Info for {Name}";
}
