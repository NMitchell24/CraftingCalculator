using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// Searches every component and every blueprint that <see cref="Blueprint"/> can nest, and adds them to it or
/// changes their quantity in place. Each change applies to <see cref="Blueprint"/> immediately.
/// </summary>
public partial class AddPartsDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRecordService RecordService { get; set; } = null!;

    /// <summary>The blueprint being edited.</summary>
    [Parameter, EditorRequired] public BlueprintModel Blueprint { get; set; } = null!;

    /// <summary>Raised after every change to <see cref="Blueprint"/>'s parts.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    private List<IBaseDataRecord> _candidates = [];
    private RecordFilter _filter = RecordFilter.Empty;

    private List<IBaseDataRecord> FilteredCandidates => RecordFilterProcessor.Apply(_candidates, _filter);

    protected override async Task OnInitializedAsync()
    {
        List<IBaseDataRecord> components = await RecordService.GetRecordsAsync(DataType.Component);

        // Leaving out this blueprint and every blueprint that already nests it is the whole cycle
        // guard: a loop the crafting tree has no bottom to can only be written by picking one of
        // those, so the list never offers one.
        IEnumerable<IBaseDataRecord> blueprints = (await RecordService.GetRecordsAsync(DataType.Blueprint))
            .OfType<BlueprintModel>()
            .Where(candidate => !BlueprintProcessor.WouldCreateCycle(Blueprint, candidate));

        _candidates = [.. components.Concat(blueprints).OrderBy(record => record.Name, StringComparer.CurrentCultureIgnoreCase)];
    }

    private void OnFilterChanged(RecordFilter filter) => _filter = filter;

    private Task AddAsync(IBaseDataRecord candidate)
    {
        BlueprintPartProcessor.Add(Blueprint, candidate, 1);
        return OnChanged.InvokeAsync();
    }

    private Task StepAsync(IBaseQuantityRecord part, long step)
    {
        BlueprintPartProcessor.Step(Blueprint, part, step);
        return OnChanged.InvokeAsync();
    }

    private Task SetQuantityAsync(IBaseQuantityRecord part, long quantity)
    {
        // Typing 0, or clearing the field and leaving it, keeps the part: removing it would swap the row back to Add
        // under the field being edited. Stepping below 1 removes it, and the editor's Save warns about any left at 0.
        part.Quantity = quantity;
        return OnChanged.InvokeAsync();
    }

    private void Close() => MudDialog.Close();
}
