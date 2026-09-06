using CraftingCalculator.Application.Common.Interfaces;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// Stub for the future Calculate screen (PR 6). Its job right now is to prove the full stack -
/// Razor page -&gt; Application service -&gt; Infrastructure DAO -&gt; SQLite - actually works end to
/// end before any real screen is built on top of the shell.
/// </summary>
public partial class Calculate : ComponentBase
{
    [Inject] private IRecipeFilterService RecipeFilterService { get; set; } = null!;
    [Inject] private IIngredientService IngredientService { get; set; } = null!;
    [Inject] private IRecipeService RecipeService { get; set; } = null!;

    private bool _loading = true;
    private List<string> _filterNames = [];
    private int _ingredientCount;
    private int _recipeCount;

    protected override async Task OnInitializedAsync()
    {
        _filterNames = [.. (await RecipeFilterService.GetRecipeFiltersAsync()).Select(f => f.Name ?? "")];
        _ingredientCount = (await IngredientService.GetAllIngredientsAsync()).Count;
        _recipeCount = (await RecipeService.GetAllRecipesAsync()).Count;
        _loading = false;
    }
}
