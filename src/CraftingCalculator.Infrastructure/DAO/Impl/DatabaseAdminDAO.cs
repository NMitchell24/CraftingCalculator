using CraftingCalculator.Application.Common.Interfaces.DAO;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class DatabaseAdminDAO(DatasetScopedContextFactory contextFactory) : IDatabaseAdminDAO
{
    public async Task DeleteAllDataAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        // Cascade delete removes BlueprintComponents/BlueprintChildren/FavoriteBlueprints as their parent
        // Blueprints, Components, and Favorites are removed. Each of these reaches only the selected
        // dataset's rows: the query filter applies to ExecuteDelete the same way it does to a read.
        await context.Blueprints.ExecuteDeleteAsync();
        await context.Components.ExecuteDeleteAsync();
        await context.Favorites.ExecuteDeleteAsync();
        await context.Categories.ExecuteDeleteAsync();
    }
}
