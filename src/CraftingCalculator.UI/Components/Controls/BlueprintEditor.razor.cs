using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
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
    [Inject] private IRecordService RecordService { get; set; } = null!;

    private List<IBaseQuantityRecord> Parts => BlueprintPartProcessor.GetParts(Model);

    private async Task OpenAddPartsAsync()
    {
        List<IBaseDataRecord> components = await RecordService.GetRecordsAsync(DataType.Component);

        // Leaving out this blueprint and every blueprint that already nests it is the whole cycle
        // guard: a loop the crafting tree has no bottom to can only be written by picking one of
        // those, so the list never offers one.
        IEnumerable<IBaseDataRecord> blueprints = (await RecordService.GetRecordsAsync(DataType.Blueprint))
            .OfType<BlueprintModel>()
            .Where(candidate => !BlueprintProcessor.WouldCreateCycle(Model, candidate));

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
            { dialog => dialog.Records, [.. components.Concat(blueprints).OrderBy(record => record.Name, StringComparer.CurrentCultureIgnoreCase)] },
            { dialog => dialog.Target, this },
            { dialog => dialog.FilterList, FilterList.RequirementsPicker }
        };

        await DialogService.ShowAsync<RecordPickerDialog>("Add requirements", parameters, options);
    }

    public IBaseQuantityRecord? Find(IBaseDataRecord record) => BlueprintPartProcessor.FindPart(Model, record);

    public async Task AddAsync(IBaseDataRecord record)
    {
        BlueprintPartProcessor.Add(Model, record, 1);
        await NotifyChangedAsync();
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
