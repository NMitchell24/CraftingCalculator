using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IFavoriteService
{
    /// <summary>
    /// Returns every saved favorite, name-ordered, each carrying its <see cref="RecipeFavorite.RecipeCount"/>.
    /// </summary>
    Task<List<RecipeFavorite>> GetAllFavoritesAsync();

    /// <summary>
    /// Saves or updates the favorite (any existing favorite of the same name is replaced first),
    /// then saves <paramref name="quantities"/> as its recipe quantities.
    /// </summary>
    Task SaveFavoriteAsync(RecipeFavorite favorite, List<RecipeQuantity> quantities);

    /// <summary>Renames the favorite, keeping its saved recipe quantities.</summary>
    Task RenameFavoriteAsync(RecipeFavorite favorite, string newName);

    Task DeleteFavoriteAsync(RecipeFavorite favorite);

    Task<bool> DoesFavoriteExistAsync(string? name);

    Task<List<RecipeQuantity>> GetRecipeQuantitiesForFavoriteAsync(RecipeFavorite? favorite);
}
