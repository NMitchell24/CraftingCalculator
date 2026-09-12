using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// The Category field shared by <see cref="BlueprintEditor" /> and <see cref="ComponentEditor" />.
/// Clearing it files the record under no category.
/// </summary>
public partial class CategorySelect : ComponentBase
{
    /// <summary>The record being filed. The chosen category is written straight to it.</summary>
    [Parameter, EditorRequired] public ICategorizedRecord Model { get; set; } = null!;

    /// <summary>Raised on every change, so the hosting editor can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [Inject] private IRecordService RecordService { get; set; } = null!;

    private List<CategoryModel> _categories = [];
    private int? _categoryId;

    protected override async Task OnInitializedAsync()
    {
        _categoryId = Model.Category?.Id;
        _categories = [.. (await RecordService.GetRecordsAsync(DataType.Category)).Cast<CategoryModel>()];
    }

    private async Task OnCategoryChangedAsync(int? categoryId)
    {
        _categoryId = categoryId;
        Model.Category = _categories.FirstOrDefault(category => category.Id == categoryId);

        await OnChanged.InvokeAsync();
    }
}
