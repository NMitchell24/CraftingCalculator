using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class BlueprintPickerDialog : ComponentBase
{
    /// <summary>Label of the chip that clears the category filter.</summary>
    private const string AllCategoriesLabel = "All";

    // A persisted category always has an id of 1 or higher, so this sentinel cannot collide with one -
    // including a category the user names "All". MudChipSet also resets SelectedValue to default(int)
    // when the selected chip is clicked again, which lands on this same "no filter" value.
    private const int AllCategoriesId = 0;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;
    [Inject] private ICategoryService CategoryService { get; set; } = null!;

    private List<BlueprintModel> _blueprints = [];
    private List<CategoryModel> _categories = [];
    private IReadOnlyCollection<BlueprintModel> _selected = [];
    private string _search = "";
    private int _selectedCategoryId = AllCategoriesId;

    private List<BlueprintModel> _filteredBlueprints =>
        [.. _blueprints.Where(blueprint =>
            (_selectedCategoryId == AllCategoriesId || blueprint.Category?.Id == _selectedCategoryId) &&
            (string.IsNullOrWhiteSpace(_search) || (blueprint.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false)))];

    protected override async Task OnInitializedAsync()
    {
        _blueprints = await BlueprintService.GetAllBlueprintsAsync();
        _categories = await CategoryService.GetCategoriesAsync();
    }

    private void OnCategoryChanged(int categoryId) => _selectedCategoryId = categoryId;

    private void Confirm() => MudDialog.Close(DialogResult.Ok(_selected));

    private void Cancel() => MudDialog.Cancel();
}
