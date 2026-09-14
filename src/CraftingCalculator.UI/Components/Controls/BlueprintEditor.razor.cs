using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BlueprintEditor : ComponentBase
{
    [Parameter, EditorRequired] public BlueprintModel Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;

    private List<IBaseQuantityRecord> Parts => BlueprintPartProcessor.GetParts(Model);

    private Task OpenAddPartsAsync()
    {
        DialogOptions options = new()
        {
            FullScreen = Breakpoint == Breakpoint.Xs,
            MaxWidth = MaxWidth.Small,
            CloseOnEscapeKey = true
        };

        // The dialog edits Model directly and reports each change as it happens, so there is no result to
        // await: the parts list behind it is already current whichever way the dialog is closed.
        DialogParameters<AddPartsDialog> parameters = new()
        {
            { dialog => dialog.Blueprint, Model },
            { dialog => dialog.OnChanged, EventCallback.Factory.Create(this, NotifyChangedAsync) }
        };

        return DialogService.ShowAsync<AddPartsDialog>("Add requirements", parameters, options);
    }

    private async Task StepAsync(IBaseQuantityRecord part, long step)
    {
        BlueprintPartProcessor.Step(Model, part, step);
        await NotifyChangedAsync();
    }

    private async Task SetQuantityAsync(IBaseQuantityRecord part, long quantity)
    {
        // Typing 0, or clearing the field and leaving it, keeps the part: removing it would pull the row out from
        // under the field being edited. Stepping below 1 and Delete remove it, and Save warns about any left at 0.
        part.Quantity = quantity;
        await NotifyChangedAsync();
    }

    private async Task RemoveAsync(IBaseQuantityRecord part)
    {
        BlueprintPartProcessor.Remove(Model, part);
        await NotifyChangedAsync();
    }

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
