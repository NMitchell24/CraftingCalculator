using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IRecipeFavoritesDAO
{
    Task<List<RecipeFavorite>> GetAllAsync();

    /// <summary>
    /// Looks up a favorite by name (names are the de facto key). Returns an empty
    /// <see cref="RecipeFavorite"/>, never null, when <paramref name="name"/> is null.
    /// </summary>
    Task<RecipeFavorite?> GetByNameAsync(string? name);

    /// <summary>
    /// Replaces any existing favorite with the same name (if <paramref name="favorite"/>.Id is 0) or
    /// updates the existing record by id, then replaces its saved recipe quantities with
    /// <paramref name="quantities"/>. Returns the saved favorite with its assigned Id.
    /// </summary>
    Task<RecipeFavorite> SaveAsync(RecipeFavorite favorite, List<RecipeQuantity> quantities);

    /// <summary>
    /// Deletes the favorite. Its saved recipe quantities are removed by the database's cascade
    /// delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);

    Task<List<RecipeQuantity>> GetRecipeQuantitiesAsync(int favoriteId);
}
