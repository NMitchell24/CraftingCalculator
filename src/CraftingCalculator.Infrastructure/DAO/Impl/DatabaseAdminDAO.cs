using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class DatabaseAdminDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IDatabaseAdminDAO
{
    public async Task DeleteAllDataAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Cascade delete removes BlueprintComponents/BlueprintChildren/FavoriteBlueprints as their parent
        // Blueprints, Components, and Favorites are removed.
        await context.Blueprints.ExecuteDeleteAsync();
        await context.Components.ExecuteDeleteAsync();
        await context.Favorites.ExecuteDeleteAsync();

        // The seeded "All" category is preserved - only the categories a user created are removed.
        await context.Categories.Where(category => category.Id != DatabaseSeedConstants.AllCategoryId).ExecuteDeleteAsync();
    }
}
