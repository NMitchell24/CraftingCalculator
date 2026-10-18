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
    /// updates the existing record by id, then replaces its saved blueprint quantities and already crafted
    /// blueprints with <paramref name="batch"/>'s. Returns the saved favorite with its assigned Id.
    /// </summary>
    Task<BlueprintFavorite> SaveAsync(BlueprintFavorite favorite, Batch batch);

    /// <summary>Changes the favorite's name. Its saved blueprint quantities are left as they are.</summary>
    Task RenameAsync(int id, string name);

    /// <summary>
    /// Deletes every favorite in <paramref name="ids"/>. Their saved blueprint quantities and already crafted
    /// blueprints are removed by the database's cascade delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(IEnumerable<int> ids);

    /// <summary>
    /// The batch saved as the favorite with <paramref name="favoriteId"/>, each blueprint built out with its parts. A
    /// saved quantity or already crafted mark naming a blueprint outside the current dataset is left out.
    /// </summary>
    Task<Batch> GetBatchAsync(int favoriteId);
}
