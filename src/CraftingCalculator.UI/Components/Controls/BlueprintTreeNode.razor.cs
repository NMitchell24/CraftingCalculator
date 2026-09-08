using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BlueprintTreeNode : ComponentBase
{
    [Parameter, EditorRequired] public BlueprintNode Node { get; set; } = null!;

    /// <summary>The path of the parent node, empty for a tree root. See <see cref="Path"/>.</summary>
    [Parameter] public string ParentPath { get; set; } = "";

    [Inject] private CraftState State { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    /// <summary>
    /// This node's position in the tree, e.g. "/Frame/Bracket" - distinct from <see cref="Node"/>.Id
    /// (which is just this node's own name) so that CraftState can track expansion per tree
    /// position rather than per blueprint/component name. The same blueprint can appear more than once in
    /// one tree (standalone in the batch and nested inside another batch blueprint); keying by Id alone
    /// would make every occurrence share one expanded/collapsed state.
    /// </summary>
    private string Path => $"{ParentPath}/{Node.Id ?? Node.Name}";

    /// <summary>
    /// The step's production time, shown at the end of the row, or null to leave the row unadorned.
    /// </summary>
    // An instant step is the common case in games with no crafting timers, and labelling every row
    // "Instant" would bury the handful of rows that do take time.
    private string? EndText => Node.ProductionTime > TimeSpan.Zero ? DurationProcessor.Format(Node.ProductionTime) : null;

    private Task OpenDetailAsync()
    {
        DialogParameters<CraftStepDialog> parameters = new() { { dialog => dialog.Node, Node } };
        DialogOptions options = new() { MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };

        return DialogService.ShowAsync<CraftStepDialog>(Node.Name, parameters, options);
    }
}
