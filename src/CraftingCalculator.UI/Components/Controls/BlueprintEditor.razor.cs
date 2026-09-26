using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BlueprintEditor : ComponentBase, IRecordPickerTarget
{
    [Parameter, EditorRequired] public BlueprintModel Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IComponentService ComponentService { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;
    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;

    private List<IBaseQuantityRecord> Parts => BlueprintPartProcessor.GetParts(Model);

    private Task OpenAddPartsAsync() => Guard.RunAsync(
        "BlueprintEditor.OpenAddParts",
        "I couldn't open the requirements picker. Your edits are still here.",
        ShowAddPartsAsync);

    private async Task ShowAddPartsAsync()
    {
        List<ComponentModel> components = await Task.Run(ComponentService.GetAllComponentsAsync);

        // The only cycle guard: the list never offers a blueprint that would close a loop, so none can be picked.
        List<BlueprintSummary> blueprints =
            await Task.Run(() => BlueprintService.GetNestableBlueprintSummariesAsync(Model.Id));

        DialogOptions options = new()
        {
            FullScreen = Breakpoint == Breakpoint.Xs,
            MaxWidth = MaxWidth.Small,
            CloseOnEscapeKey = true
        };

        // The dialog edits Model through this editor as each change happens, so there is no result to await: the
        // parts list behind it is already current whichever way the dialog is closed.
        DialogParameters<RecordPickerDialog> parameters = new()
        {
            { dialog => dialog.Title, "Add requirements" },
            { dialog => dialog.Records, [.. components.Concat<IBaseDataRecord>(blueprints).OrderBy(record => record.Name, StringComparer.CurrentCultureIgnoreCase)] },
            { dialog => dialog.Target, this },
            { dialog => dialog.FilterList, FilterList.RequirementsPicker }
        };

        await DialogService.ShowAsync<RecordPickerDialog>("Add requirements", parameters, options);
    }

    public IBaseQuantityRecord? Find(IBaseDataRecord record) => BlueprintPartProcessor.FindPart(Model, record);

    public async Task AddAsync(IBaseDataRecord record)
    {
        // Blueprints are listed as summaries, and a part holds the full blueprint, as one read back from the database
        // does. Null only when the blueprint was deleted after the picker opened, which leaves nothing to add.
        IBaseDataRecord? part = record is BlueprintSummary
            ? await Task.Run(() => BlueprintService.GetBlueprintByIdAsync(record.Id))
            : record;

        if (part is not null)
        {
            BlueprintPartProcessor.Add(Model, part, 1);
            await NotifyChangedAsync();
        }
    }

    public async Task StepAsync(IBaseQuantityRecord entry, long step)
    {
        BlueprintPartProcessor.Step(Model, entry, step);
        await NotifyChangedAsync();
    }

    public async Task SetQuantityAsync(IBaseQuantityRecord entry, long quantity)
    {
        // Typing 0, or clearing the field and leaving it, keeps the part: removing it would pull the row out from
        // under the field being edited. Stepping down from 0 and Delete remove it, and Save warns about any left at 0.
        entry.Quantity = quantity;
        await NotifyChangedAsync();
    }

    public async Task RemoveAsync(IBaseQuantityRecord entry)
    {
        BlueprintPartProcessor.Remove(Model, entry);
        await NotifyChangedAsync();
    }

    private static string RemoveLabel(IBaseQuantityRecord part) => $"Remove {part.Name}";

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
