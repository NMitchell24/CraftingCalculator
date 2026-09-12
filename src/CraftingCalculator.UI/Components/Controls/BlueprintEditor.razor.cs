using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BlueprintEditor : ComponentBase
{
    [Parameter, EditorRequired] public BlueprintModel Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [Inject] private IRecordService RecordService { get; set; } = null!;

    private List<IBaseDataRecord> _components = [];
    private List<IBaseDataRecord> _childBlueprintCandidates = [];

    private DataType _partType = DataType.Component;
    private IBaseDataRecord? _selectedPart;
    private long _quantityToAdd = 1;

    private List<IBaseQuantityRecord> Parts => BlueprintPartProcessor.GetParts(Model);

    protected override async Task OnInitializedAsync()
    {
        _components = await RecordService.GetRecordsAsync(DataType.Component);

        // Leaving out this blueprint and every blueprint that already nests it is the whole cycle
        // guard: a loop the crafting tree has no bottom to can only be written by picking one of
        // those, so the list never offers one.
        _childBlueprintCandidates =
            [.. (await RecordService.GetRecordsAsync(DataType.Blueprint))
                .OfType<BlueprintModel>()
                .Where(candidate => !BlueprintProcessor.WouldCreateCycle(Model, candidate))];
    }

    private Task<IEnumerable<IBaseDataRecord>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        List<IBaseDataRecord> source = _partType == DataType.Blueprint ? _childBlueprintCandidates : _components;

        return Task.FromResult<IEnumerable<IBaseDataRecord>>(source.Where(record =>
            string.IsNullOrWhiteSpace(search) || (record.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)));
    }

    private void OnPartTypeChanged(DataType type)
    {
        _partType = type;
        _selectedPart = null;
        _quantityToAdd = 1;
    }

    private async Task AddComponentAsync()
    {
        BlueprintPartProcessor.Add(Model, _selectedPart, _quantityToAdd);

        _selectedPart = null;
        _quantityToAdd = 1;

        await NotifyChangedAsync();
    }

    private async Task SetQuantityAsync(IBaseQuantityRecord part, long quantity)
    {
        BlueprintPartProcessor.SetQuantity(Model, part, quantity);
        await NotifyChangedAsync();
    }

    private async Task RemoveAsync(IBaseQuantityRecord part)
    {
        BlueprintPartProcessor.Remove(Model, part);
        await NotifyChangedAsync();
    }

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
