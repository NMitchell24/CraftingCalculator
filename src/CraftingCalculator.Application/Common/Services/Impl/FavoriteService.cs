using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class FavoriteService(IBlueprintFavoritesDAO dao) : IFavoriteService
{
    public Task<List<BlueprintFavorite>> GetAllFavoritesAsync() => dao.GetAllAsync();

    public Task SaveFavoriteAsync(BlueprintFavorite favorite, List<BlueprintQuantity> quantities)
        => dao.SaveAsync(favorite, quantities);

    public Task RenameFavoriteAsync(BlueprintFavorite favorite, string newName) => dao.RenameAsync(favorite.Id, newName);

    public Task DeleteFavoriteAsync(BlueprintFavorite favorite) => dao.DeleteAsync(favorite.Id);

    public async Task<bool> DoesFavoriteExistAsync(string? name) => await dao.GetByNameAsync(name) != null;

    public Task<List<BlueprintQuantity>> GetBlueprintQuantitiesForFavoriteAsync(BlueprintFavorite? favorite)
        => favorite != null ? dao.GetBlueprintQuantitiesAsync(favorite.Id) : Task.FromResult(new List<BlueprintQuantity>());
}
