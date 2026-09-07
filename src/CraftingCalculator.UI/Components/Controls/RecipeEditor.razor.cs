using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class RecipeEditor : ComponentBase
{
    [Parameter, EditorRequired] public Recipe Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [Inject] private ILibraryService LibraryService { get; set; } = null!;

    private List<RecipeFilter> _filters = [];
    private List<IBaseDataRecord> _ingredients = [];
    private List<IBaseDataRecord> _childRecipeCandidates = [];

    private DataType _componentType = DataType.Ingredient;
    private IBaseDataRecord? _selectedComponent;
    private long _quantityToAdd = 1;
    private int? _filterId;

    private List<IBaseQuantityRecord> Components => RecipeComponentProcessor.GetComponents(Model);

    protected override async Task OnInitializedAsync()
    {
        _filterId = Model.Filter?.Id;

        _filters = [.. (await LibraryService.GetRecordsAsync(DataType.RecipeFilter)).Cast<RecipeFilter>()];
        _ingredients = await LibraryService.GetRecordsAsync(DataType.Ingredient);

        // Ports ConfigureRecipesViewModel.RecipeSelectedType: a recipe cannot be its own component.
        // Nothing guards a longer A -> B -> A cycle here either, matching the WPF app - the depth cap
        // in RecipeProcessor is what keeps that catchable.
        _childRecipeCandidates =
            [.. (await LibraryService.GetRecordsAsync(DataType.Recipe)).Where(r => r.Id != Model.Id)];
    }

    private Task<IEnumerable<IBaseDataRecord>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        List<IBaseDataRecord> source = _componentType == DataType.Recipe ? _childRecipeCandidates : _ingredients;

        return Task.FromResult<IEnumerable<IBaseDataRecord>>(source.Where(r =>
            string.IsNullOrWhiteSpace(search) || (r.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false)));
    }

    private void OnComponentTypeChanged(DataType type)
    {
        _componentType = type;
        _selectedComponent = null;
        _quantityToAdd = 1;
    }

    private async Task OnFilterChangedAsync(int? filterId)
    {
        _filterId = filterId;
        Model.Filter = _filters.FirstOrDefault(f => f.Id == filterId);

        await NotifyChangedAsync();
    }

    private async Task AddComponentAsync()
    {
        RecipeComponentProcessor.Add(Model, _selectedComponent, _quantityToAdd);

        _selectedComponent = null;
        _quantityToAdd = 1;

        await NotifyChangedAsync();
    }

    private async Task SetQuantityAsync(IBaseQuantityRecord component, long quantity)
    {
        RecipeComponentProcessor.SetQuantity(Model, component, quantity);
        await NotifyChangedAsync();
    }

    private async Task RemoveAsync(IBaseQuantityRecord component)
    {
        RecipeComponentProcessor.Remove(Model, component);
        await NotifyChangedAsync();
    }

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
