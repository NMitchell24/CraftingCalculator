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

    /// <summary>This node's position in the tree. See <see cref="CraftState.PathOf"/>.</summary>
    private string Path => CraftState.PathOf(ParentPath, Node);

    /// <summary>
    /// The row label: the step's name followed by how many of it this step covers.
    /// </summary>
    // A yield above 1 makes the craft count the actionable number - the user performs crafts, not items -
    // so those rows count crafts and take the asterisk that StepsTree's legend explains. The quantity
    // stays one tap away in InfoDialog.
    private string Label => BlueprintProcessor.CountsByCraft(Node)
        ? $"{Node.Name} x{Node.Crafts}*"
        : $"{Node.Name} x{Node.Quantity}";

    private string? LabelClass => BlueprintProcessor.CountsByCraft(Node) ? "steps-tree-crafts" : null;

    /// <summary>
    /// The step's production time, shown at the end of the row, or null to leave the row unadorned.
    /// </summary>
    // An instant step is the common case in games with no crafting timers, and labelling every row
    // "Instant" would bury the handful of rows that do take time.
    private string? EndText => Node.ProductionTime > TimeSpan.Zero ? DurationProcessor.Format(Node.ProductionTime) : null;

    private Task OpenDetailAsync() => InfoDialog.ShowAsync(DialogService, Node);
}
