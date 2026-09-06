using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class RecipePickerDialog : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
    [Inject] private IRecipeService RecipeService { get; set; } = null!;
    [Inject] private IRecipeFilterService RecipeFilterService { get; set; } = null!;

    private List<Recipe> _recipes = [];
    private List<RecipeFilter> _filters = [];
    private IReadOnlyCollection<Recipe> _selected = [];
    private string _search = "";
    private string _selectedFilterName = RecipeFilter.ALL;

    private List<Recipe> _filteredRecipes =>
        [.. _recipes.Where(r =>
            (_selectedFilterName == RecipeFilter.ALL || r.Filter?.Name == _selectedFilterName) &&
            (string.IsNullOrWhiteSpace(_search) || (r.Name?.Contains(_search, StringComparison.OrdinalIgnoreCase) ?? false)))];

    protected override async Task OnInitializedAsync()
    {
        _recipes = await RecipeService.GetAllRecipesAsync();

        // The seeded "All" row (RecipeFilter.ALL) is the sentinel this dialog already renders as its
        // own first chip, not a real category - excluded here the same way WPF dropped it positionally
        // from ConfigureRecipesViewModel's filter list.
        _filters = [.. (await RecipeFilterService.GetRecipeFiltersAsync()).Where(f => f.Name != RecipeFilter.ALL)];
    }

    private void OnFilterChanged(string filterName) => _selectedFilterName = filterName;

    private void Confirm() => MudDialog.Close(DialogResult.Ok(_selected));

    private void Cancel() => MudDialog.Cancel();
}
