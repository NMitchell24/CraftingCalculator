using System.Runtime.CompilerServices;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
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

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

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
    // An instant step is the common case in games with no crafting timers, and labeling every row
    // "Instant" would bury the handful of rows that do take time.
    private string? EndText => SelectedDataset.Settings.UseCraftTime && Node.ProductionTime > TimeSpan.Zero
        ? DurationProcessor.Format(Node.ProductionTime)
        : null;

    // Set only by this row's own arrow closing it, so the children stay for MudCollapse's slide. Every render its
    // parent drives - a batch change, Collapse all, the parent itself opening or closing - clears it, and a hidden
    // subtree goes with the next one of those.
    private bool _keepChildren;

    // What the last render drew from. Node is a record, so Blazor treats every parent render as a parameter change
    // and re-renders the row; MudCollapse re-renders its content when it first renders open and again when its
    // slide ends, which without this check re-renders the whole subtree once per open ancestor. On Expand all
    // that was thousands of subtree renders queued behind the click, seconds of work after the tree looked done.
    private RenderedState _rendered;

    // The first render never consults ShouldRender, and draws from exactly this state.
    protected override void OnInitialized()
    {
        _rendered = CurrentState();
    }

    protected override void OnParametersSet()
    {
        _keepChildren = false;
    }

    protected override bool ShouldRender()
    {
        RenderedState current = CurrentState();
        if (current == _rendered)
        {
            return false;
        }

        _rendered = current;
        return true;
    }

    // Node by reference: a recalculated batch builds new nodes, and an unchanged one keeps its instance. The
    // setting stands in for EndText, which is otherwise a function of the node alone, so a skipped render formats
    // no duration.
    private RenderedState CurrentState() =>
        new(Node, ParentPath, State.IsExpanded(Path), _keepChildren, SelectedDataset.Settings.UseCraftTime);

    private readonly record struct RenderedState(
        BlueprintNode Node, string ParentPath, bool Expanded, bool KeepChildren, bool UseCraftTime)
    {
        public bool Equals(RenderedState other) =>
            ReferenceEquals(Node, other.Node) && ParentPath == other.ParentPath && Expanded == other.Expanded
            && KeepChildren == other.KeepChildren && UseCraftTime == other.UseCraftTime;

        public override int GetHashCode() =>
            HashCode.Combine(RuntimeHelpers.GetHashCode(Node), ParentPath, Expanded, KeepChildren, UseCraftTime);
    }

    private void OnExpandedChanged(bool expanded)
    {
        _keepChildren = !expanded;
        State.ToggleExpanded(Path);
    }

    private Task OpenDetailAsync() => InfoDialog.ShowAsync(DialogService, Node);
}
