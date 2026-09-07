using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IRecipeFilterDAO
{
    /// <summary>
    /// Returns all filters with the <see cref="RecipeFilter.ALL"/> filter first, then alphabetically.
    /// </summary>
    Task<List<RecipeFilter>> GetAllAsync();

    Task<RecipeFilter?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the filter if <see cref="RecipeFilter.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved filter with its assigned <see cref="RecipeFilter.Id"/>.
    /// </summary>
    Task<RecipeFilter> SaveAsync(RecipeFilter filter);

    /// <summary>
    /// Deletes the filter. Recipes referencing it have their filter reference cleared by the
    /// database's SetNull cascade rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);
}
