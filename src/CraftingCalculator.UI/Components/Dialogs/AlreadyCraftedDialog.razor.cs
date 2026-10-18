using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// Lists every blueprint nested in the Craft screen's batch, each with a switch that marks it as already crafted for
/// the batch. Each switch applies to <see cref="CraftState"/> as it flips, so the dialog has no result.
/// </summary>
public partial class AlreadyCraftedDialog : ComponentBase
{
    // The pitch of a one-line row at 100% text: body1's line plus the switch's padding and the row's own. Virtualize
    // re-measures from the rows it renders, so a caption or a name that wraps only makes the scrollbar approximate.
    private const float RowHeight = 50;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private CraftState State { get; set; } = null!;

    // Read once: the batch can't change while the dialog covers the Craft screen, and a mark never changes what's
    // nested, only which rows are disabled.
    private IReadOnlyList<BlueprintModel> _candidates = [];
    private List<BlueprintModel> _filtered = [];

    protected override void OnInitialized()
    {
        _candidates = State.AlreadyCraftedCandidates;
        _filtered = [.. _candidates];
    }

    private void OnFilterChanged(RecordFilter filter) => _filtered = RecordFilterProcessor.Apply(_candidates, filter);

    private void Close() => MudDialog.Close();
}
