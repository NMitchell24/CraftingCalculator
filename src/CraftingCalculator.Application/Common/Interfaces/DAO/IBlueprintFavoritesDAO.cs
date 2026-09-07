using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IBlueprintFavoritesDAO
{
    /// <summary>
    /// Returns every favorite, name-ordered. Each carries its <see cref="BlueprintFavorite.BlueprintCount"/>;
    /// the other members of this interface leave that count at 0.
    /// </summary>
    Task<List<BlueprintFavorite>> GetAllAsync();

    /// <summary>
    /// Looks up a favorite by name (names are the de facto key). Returns an empty
    /// <see cref="BlueprintFavorite"/>, never null, when <paramref name="name"/> is null.
    /// </summary>
    Task<BlueprintFavorite?> GetByNameAsync(string? name);

    /// <summary>
    /// Replaces any existing favorite with the same name (if <paramref name="favorite"/>.Id is 0) or
    /// updates the existing record by id, then replaces its saved blueprint quantities with
    /// <paramref name="quantities"/>. Returns the saved favorite with its assigned Id.
    /// </summary>
    Task<BlueprintFavorite> SaveAsync(BlueprintFavorite favorite, List<BlueprintQuantity> quantities);

    /// <summary>Changes the favorite's name. Its saved blueprint quantities are left as they are.</summary>
    Task RenameAsync(int id, string name);

    /// <summary>
    /// Deletes the favorite. Its saved blueprint quantities are removed by the database's cascade
    /// delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);

    Task<List<BlueprintQuantity>> GetBlueprintQuantitiesAsync(int favoriteId);
}
