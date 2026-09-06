using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FavoriteEntity = CraftingCalculator.Domain.Entities.Favorite;
using FavoriteRecipeEntity = CraftingCalculator.Domain.Entities.FavoriteRecipe;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class RecipeFavoritesDAO(IDbContextFactory<CraftingDataContext> contextFactory, IRecipeDAO recipeDAO) : IRecipeFavoritesDAO
{
    public async Task<List<RecipeFavorite>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<FavoriteEntity> entities = await context.Favorites
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<RecipeFavorite?> GetByNameAsync(string? name)
    {
        if (name == null)
        {
            return new RecipeFavorite();
        }

        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        FavoriteEntity? entity = await context.Favorites.AsNoTracking().FirstOrDefaultAsync(f => f.Name == name);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<RecipeFavorite> SaveAsync(RecipeFavorite favorite, List<RecipeQuantity> quantities)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        FavoriteEntity? entity = favorite.Id > 0
            ? await context.Favorites.FirstOrDefaultAsync(f => f.Id == favorite.Id)
            : await context.Favorites.FirstOrDefaultAsync(f => f.Name == favorite.Name);

        if (entity == null)
        {
            entity = new FavoriteEntity();
            context.Favorites.Add(entity);
        }

        entity.Name = favorite.Name ?? "";

        await context.SaveChangesAsync();

        // Replace the saved quantities wholesale rather than diffing them.
        await context.FavoriteRecipes.Where(fr => fr.FavoriteId == entity.Id).ExecuteDeleteAsync();

        foreach (RecipeQuantity rq in quantities)
        {
            context.FavoriteRecipes.Add(new FavoriteRecipeEntity
            {
                FavoriteId = entity.Id,
                RecipeId = rq.Recipe.Id,
                Quantity = rq.Quantity
            });
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Favorites.Where(f => f.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<RecipeQuantity>> GetRecipeQuantitiesAsync(int favoriteId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<FavoriteRecipeEntity> rows = await context.FavoriteRecipes
            .AsNoTracking()
            .Where(fr => fr.FavoriteId == favoriteId)
            .ToListAsync();

        List<RecipeQuantity> result = [];
        foreach (FavoriteRecipeEntity row in rows)
        {
            Recipe? recipe = await recipeDAO.GetByIdAsync(row.RecipeId);
            if (recipe != null)
            {
                // Id is hardcoded to 0 here to match the old RecipeFavoriteService quirk: a
                // RecipeQuantity reconstructed from a favorite never carries the FavoriteRecipe row's
                // own Id.
                result.Add(new RecipeQuantity(recipe, row.Quantity, 0));
            }
        }

        return result;
    }

    private static RecipeFavorite ToModel(FavoriteEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name
    };
}
