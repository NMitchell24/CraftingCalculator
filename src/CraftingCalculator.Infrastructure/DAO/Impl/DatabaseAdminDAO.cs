using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class DatabaseAdminDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IDatabaseAdminDAO
{
    public async Task DeleteAllDataAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Cascade delete removes RecipeIngredients/RecipeChildren/FavoriteRecipes as their parent
        // Recipes, Ingredients, and Favorites are removed.
        await context.Recipes.ExecuteDeleteAsync();
        await context.Ingredients.ExecuteDeleteAsync();
        await context.Favorites.ExecuteDeleteAsync();

        // The seeded "All" filter is preserved - only the filters a user created are removed.
        await context.RecipeFilters.Where(f => f.Id != DatabaseSeedConstants.AllFilterId).ExecuteDeleteAsync();
    }
}
