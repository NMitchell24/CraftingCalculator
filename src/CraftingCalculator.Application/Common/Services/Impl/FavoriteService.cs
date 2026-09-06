using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class FavoriteService(IRecipeFavoritesDAO dao) : IFavoriteService
{
    public Task<List<RecipeFavorite>> GetAllFavoritesAsync() => dao.GetAllAsync();

    public Task SaveFavoriteAsync(RecipeFavorite favorite, List<RecipeQuantity> quantities)
        => dao.SaveAsync(favorite, quantities);

    public Task DeleteFavoriteAsync(RecipeFavorite favorite) => dao.DeleteAsync(favorite.Id);

    public async Task<bool> DoesFavoriteExistAsync(string? name) => await dao.GetByNameAsync(name) != null;

    public Task<List<RecipeQuantity>> GetRecipeQuantitiesForFavoriteAsync(RecipeFavorite? favorite)
        => favorite != null ? dao.GetRecipeQuantitiesAsync(favorite.Id) : Task.FromResult(new List<RecipeQuantity>());
}
