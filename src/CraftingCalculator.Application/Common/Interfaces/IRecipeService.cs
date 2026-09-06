using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IRecipeService
{
    Task<List<Recipe>> GetRecipesByFilterAsync(RecipeFilter filter);

    Task<Recipe?> GetRecipeByIdAsync(int id);

    Task<List<Recipe>> GetAllRecipesAsync();

    /// <summary>
    /// Saves or adds the recipe. If <see cref="Recipe.Id"/> is 0 a new one is added, otherwise the
    /// existing record (including its components) is updated.
    /// </summary>
    Task SaveRecipeAsync(Recipe? recipe);

    Task DeleteRecipeAsync(Recipe? recipe);

    /// <summary>
    /// Flattens the recipe's own ingredients and every (recursively) nested child recipe's
    /// ingredients into one combined <see cref="IngredientMap"/>.
    /// </summary>
    IngredientMap GetFlattenedIngredients(Recipe recipe);

    /// <summary>
    /// Builds the recipe's component breakdown as a tree, scaled by <paramref name="quantity"/>.
    /// </summary>
    RecipeTree GetRecipeTree(Recipe recipe, long quantity);

    /// <summary>
    /// Builds the recipe's component breakdown as an immutable <see cref="RecipeNode"/> tree, scaled
    /// by <paramref name="quantity"/>. Use this (not <see cref="GetRecipeTree"/>) for new code.
    /// </summary>
    RecipeNode GetRecipeNode(Recipe recipe, long quantity);
}
