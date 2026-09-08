using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The detail behind one row of the Crafting Steps tree: how many the step makes, how many crafts that
/// takes, what it overproduces, and how long it runs.
/// </summary>
public partial class CraftStepDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter, EditorRequired] public BlueprintNode Node { get; set; } = null!;

    private void Close() => MudDialog.Close();
}
