using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class FavoriteService(IBlueprintFavoritesDAO dao) : IFavoriteService
{
    public Task<List<BlueprintFavorite>> GetAllFavoritesAsync() => dao.GetAllAsync();

    public Task<BlueprintFavorite> SaveFavoriteAsync(BlueprintFavorite favorite, Batch batch)
        => dao.SaveAsync(favorite, batch);

    public Task RenameFavoriteAsync(BlueprintFavorite favorite, string newName) => dao.RenameAsync(favorite.Id, newName);

    public Task DeleteFavoriteAsync(BlueprintFavorite favorite) => dao.DeleteAsync([favorite.Id]);

    public Task DeleteFavoritesAsync(IEnumerable<BlueprintFavorite> favorites) =>
        dao.DeleteAsync(favorites.Select(favorite => favorite.Id));

    public async Task<bool> DoesFavoriteExistAsync(string? name) => await dao.GetByNameAsync(name) != null;

    public Task<Batch> GetBatchForFavoriteAsync(BlueprintFavorite? favorite)
        => favorite != null ? dao.GetBatchAsync(favorite.Id) : Task.FromResult(Batch.Empty);
}
