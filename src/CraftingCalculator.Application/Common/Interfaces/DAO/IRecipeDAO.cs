using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IRecipeDAO
{
    /// <summary>
    /// Returns every recipe matching <paramref name="filter"/>, or all recipes when
    /// <paramref name="filter"/> is the <see cref="RecipeFilter.ALL"/> filter. Each recipe is
    /// returned with its <see cref="Recipe.Components"/> and <see cref="Recipe.ChildRecipes"/>
    /// populated.
    /// </summary>
    Task<List<Recipe>> GetByFilterAsync(RecipeFilter filter);

    /// <summary>
    /// Returns the recipe with its full component graph populated (components and, recursively,
    /// child recipes), or null if no recipe with this id exists.
    /// </summary>
    Task<Recipe?> GetByIdAsync(int id);

    /// <summary>
    /// Returns every recipe, each with its full component graph populated.
    /// </summary>
    Task<List<Recipe>> GetAllAsync();

    /// <summary>
    /// Adds the recipe if <see cref="Recipe.Id"/> is 0, otherwise updates the existing record
    /// (including its component and child-recipe components). Returns the saved recipe with its
    /// assigned <see cref="Recipe.Id"/>.
    /// </summary>
    Task<Recipe> SaveAsync(Recipe recipe);

    /// <summary>
    /// Deletes the recipe. Its components, any recipes that use it as a child, and any favorite
    /// entries referencing it are removed by the database's cascade delete rather than by the
    /// caller.
    /// </summary>
    Task DeleteAsync(int id);
}
