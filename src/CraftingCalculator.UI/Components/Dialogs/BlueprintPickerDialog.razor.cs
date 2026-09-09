using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class BlueprintPickerDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IBlueprintService BlueprintService { get; set; } = null!;
    [Inject] private ICategoryService CategoryService { get; set; } = null!;

    private List<BlueprintModel> _blueprints = [];
    private List<CategoryModel> _categories = [];
    private IReadOnlyCollection<BlueprintModel> _selected = [];
    private string _search = "";
    private string _selectedCategoryName = CategoryModel.All;

    private List<BlueprintModel> _filteredBlueprints =>
        [.. _blueprints.Where(blueprint =>
            (_selectedCategoryName == CategoryModel.All || blueprint.Category?.Name == _selectedCategoryName) &&
            (string.IsNullOrWhiteSpace(_search) || (blueprint.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false)))];

    protected override async Task OnInitializedAsync()
    {
        _blueprints = await BlueprintService.GetAllBlueprintsAsync();

        // The seeded "All" row is the sentinel this dialog already renders as its own first chip, not
        // a real category - excluded here the same way WPF dropped it positionally from
        // ConfigureBlueprintsViewModel's category list. Matched on id rather than name so a user category
        // of their own called "All" (which the Dataset screen lets them create) still shows up.
        _categories = [.. (await CategoryService.GetCategoriesAsync())
            .Where(category => category.Id != DatabaseSeedConstants.AllCategoryId)];
    }

    private void OnCategoryChanged(string categoryName) => _selectedCategoryName = categoryName;

    private void Confirm() => MudDialog.Close(DialogResult.Ok(_selected));

    private void Cancel() => MudDialog.Cancel();
}
