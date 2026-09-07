using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IRecipeFilterService
{
    Task<List<RecipeFilter>> GetRecipeFiltersAsync();

    Task<RecipeFilter?> GetRecipeFilterByIdAsync(int id);

    /// <summary>
    /// Saves or adds the filter. If <see cref="RecipeFilter.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveRecipeFilterAsync(RecipeFilter? filter);

    Task DeleteRecipeFilterAsync(RecipeFilter? filter);
}
