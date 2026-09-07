using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BlueprintEditor : ComponentBase
{
    [Parameter, EditorRequired] public Blueprint Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [Inject] private IDatasetService DatasetService { get; set; } = null!;

    private List<Category> _categories = [];
    private List<IBaseDataRecord> _components = [];
    private List<IBaseDataRecord> _childBlueprintCandidates = [];

    private DataType _partType = DataType.Component;
    private IBaseDataRecord? _selectedPart;
    private long _quantityToAdd = 1;
    private int? _categoryId;

    private List<IBaseQuantityRecord> Parts => BlueprintPartProcessor.GetParts(Model);

    protected override async Task OnInitializedAsync()
    {
        _categoryId = Model.Category?.Id;

        _categories = [.. (await DatasetService.GetRecordsAsync(DataType.Category)).Cast<Category>()];
        _components = await DatasetService.GetRecordsAsync(DataType.Component);

        // Ports ConfigureBlueprintsViewModel.BlueprintSelectedType: a blueprint cannot be its own part.
        // Nothing guards a longer A -> B -> A cycle here either, matching the WPF app - the depth cap
        // in BlueprintProcessor is what keeps that catchable.
        _childBlueprintCandidates =
            [.. (await DatasetService.GetRecordsAsync(DataType.Blueprint)).Where(r => r.Id != Model.Id)];
    }

    private Task<IEnumerable<IBaseDataRecord>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        List<IBaseDataRecord> source = _partType == DataType.Blueprint ? _childBlueprintCandidates : _components;

        return Task.FromResult<IEnumerable<IBaseDataRecord>>(source.Where(r =>
            string.IsNullOrWhiteSpace(search) || (r.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)));
    }

    private void OnPartTypeChanged(DataType type)
    {
        _partType = type;
        _selectedPart = null;
        _quantityToAdd = 1;
    }

    private async Task OnCategoryChangedAsync(int? categoryId)
    {
        _categoryId = categoryId;
        Model.Category = _categories.FirstOrDefault(f => f.Id == categoryId);

        await NotifyChangedAsync();
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
