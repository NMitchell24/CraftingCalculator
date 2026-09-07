using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IFavoriteService
{
    /// <summary>
    /// Returns every saved favorite, name-ordered, each carrying its <see cref="BlueprintFavorite.BlueprintCount"/>.
    /// </summary>
    Task<List<BlueprintFavorite>> GetAllFavoritesAsync();

    /// <summary>
    /// Saves or updates the favorite (any existing favorite of the same name is replaced first),
    /// then saves <paramref name="quantities"/> as its blueprint quantities.
    /// </summary>
    Task SaveFavoriteAsync(BlueprintFavorite favorite, List<BlueprintQuantity> quantities);

    /// <summary>Renames the favorite, keeping its saved blueprint quantities.</summary>
    Task RenameFavoriteAsync(BlueprintFavorite favorite, string newName);

    Task DeleteFavoriteAsync(BlueprintFavorite favorite);

    Task<bool> DoesFavoriteExistAsync(string? name);

    Task<List<BlueprintQuantity>> GetBlueprintQuantitiesForFavoriteAsync(BlueprintFavorite? favorite);
}
