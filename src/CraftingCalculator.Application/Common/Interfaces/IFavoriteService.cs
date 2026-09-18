using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IFavoriteService
{
    /// <summary>
    /// Returns every saved favorite, name-ordered, each carrying its <see cref="BlueprintFavorite.BlueprintCount"/>.
    /// </summary>
    Task<List<BlueprintFavorite>> GetAllFavoritesAsync();

    /// <summary>
    /// Saves <paramref name="quantities"/> as a favorite's blueprint quantities and returns that favorite: the one with
    /// the id of <paramref name="favorite"/>, or, when it has no id, the one with its name, which is created if no
    /// favorite has it.
    /// </summary>
    Task<BlueprintFavorite> SaveFavoriteAsync(BlueprintFavorite favorite, List<BlueprintQuantity> quantities);

    /// <summary>Renames the favorite, keeping its saved blueprint quantities.</summary>
    Task RenameFavoriteAsync(BlueprintFavorite favorite, string newName);

    Task DeleteFavoriteAsync(BlueprintFavorite favorite);

    /// <summary>Deletes every favorite in <paramref name="favorites"/>.</summary>
    Task DeleteFavoritesAsync(IEnumerable<BlueprintFavorite> favorites);

    Task<bool> DoesFavoriteExistAsync(string? name);

    Task<List<BlueprintQuantity>> GetBlueprintQuantitiesForFavoriteAsync(BlueprintFavorite? favorite);
}
