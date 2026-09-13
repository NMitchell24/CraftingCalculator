using CraftingCalculator.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// The select-all button for an expansion panel header of records. Its icon and label show how much of the panel is
/// selected; tapping it raises <see cref="OnToggle"/> without opening or closing the panel.
/// </summary>
public partial class SelectAllToggle : ComponentBase
{
    /// <summary>How much of the panel is selected.</summary>
    [Parameter, EditorRequired] public SelectionState State { get; set; }

    /// <summary>The panel's title, which names the records in the button's label.</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    [Parameter] public bool Disabled { get; set; }

    /// <summary>
    /// Raised on a tap. The owner deselects everything in the panel when <see cref="State"/> is
    /// <see cref="SelectionState.All"/>, and selects everything otherwise.
    /// </summary>
    [Parameter] public EventCallback OnToggle { get; set; }

    private string Icon => State switch
    {
        SelectionState.All => Icons.Material.Filled.CheckCircle,
        SelectionState.Some => Icons.Material.Filled.IndeterminateCheckBox,
        _ => Icons.Material.Filled.CheckCircleOutline
    };

    private Color IconColor => State switch
    {
        SelectionState.All => Color.Primary,
        SelectionState.Some => Color.Tertiary,
        _ => Color.Secondary
    };

    private string Label =>
        State == SelectionState.All ? $"Deselect all {Title.ToLowerInvariant()}" : $"Select all {Title.ToLowerInvariant()}";
}
