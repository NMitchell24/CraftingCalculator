using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// One record in a list, as a card of up to three stacked zones: the record's identity with its Info button, an
/// optional stepper zone, and an optional zone of actions. Placed inside the owner's MudListItem.
/// </summary>
public partial class RecordCard : ComponentBase
{
    /// <summary>The record's name, shown as the card's first line.</summary>
    [Parameter, EditorRequired] public string Name { get; set; } = "";

    /// <summary>
    /// The record whose details the Info button opens. Null for a row that is not a record (a favorite), which
    /// supplies <see cref="OnInfo" /> instead, or for a row with no Info button.
    /// </summary>
    [Parameter] public IBaseDataRecord? Record { get; set; }

    /// <summary>Extra identity lines, one per entry.</summary>
    [Parameter] public IReadOnlyList<string> Details { get; set; } = [];

    /// <summary>Invoked by the Info button in place of opening <see cref="Record" />'s details. The card has no Info
    /// button when this and <see cref="Record" /> are both unset.</summary>
    [Parameter] public EventCallback OnInfo { get; set; }

    /// <summary>The stepper zone: a QuantityStepper, or a picker's Add button. Null when the row has no quantity.</summary>
    [Parameter] public RenderFragment? Stepper { get; set; }

    /// <summary>The actions zone, right-aligned. Null when the row has no actions.</summary>
    [Parameter] public RenderFragment? Actions { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;

    private bool HasInfo => Record is not null || OnInfo.HasDelegate;

    private string InfoLabel => $"Info for {Name}";

    private async Task ShowInfoAsync()
    {
        if (OnInfo.HasDelegate)
        {
            await OnInfo.InvokeAsync();
            return;
        }

        // A list row carries a summary; the dialog lists the blueprint's parts, so it is handed the full one. The
        // summary stands in only when the blueprint was deleted after the list was read.
        IBaseDataRecord record = Record!;

        if (record is BlueprintSummary summary
            && await Task.Run(() => BlueprintService.GetBlueprintByIdAsync(summary.Id)) is { } blueprint)
        {
            record = blueprint;
        }

        await InfoDialog.ShowAsync(DialogService, record);
    }
}
