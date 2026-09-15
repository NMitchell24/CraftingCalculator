using CraftingCalculator.Domain.Enums;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// The header of one kind-of-record panel on the export and import screens: the kind's icon and title, then how many
/// of its records are selected, the <see cref="SelectAllToggle"/> and the expand caret. The two groups share one line
/// where they fit and stack into two otherwise. Tapping the header still opens and closes the panel; only the toggle
/// is a control of its own.
/// </summary>
public partial class TransferPanelHeader : ComponentBase
{
    /// <summary>The kind's icon, a MudBlazor icon string.</summary>
    [Parameter, EditorRequired] public string Icon { get; set; } = "";

    /// <summary>The kind's title, which also names the records in the toggle's label.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>How many of the panel's records are selected.</summary>
    [Parameter, EditorRequired] public int Count { get; set; }

    /// <summary>How much of the panel is selected, for the toggle's icon.</summary>
    [Parameter, EditorRequired] public SelectionState State { get; set; }

    /// <summary>Disables the toggle, for a panel with nothing in it.</summary>
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Whether the panel is open, which turns the caret.</summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>Raised when the toggle is tapped; see <see cref="SelectAllToggle.OnToggle"/>.</summary>
    [Parameter] public EventCallback OnToggle { get; set; }

    private string CaretClass => Expanded ? "panel-caret panel-caret-expanded" : "panel-caret";
}
