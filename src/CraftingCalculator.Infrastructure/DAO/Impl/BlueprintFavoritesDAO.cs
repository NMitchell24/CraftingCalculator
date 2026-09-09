using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintFavoritesDAO(IDbContextFactory<CraftingDataContext> contextFactory, IBlueprintDAO blueprintDAO) : IBlueprintFavoritesDAO
{
    public async Task<List<BlueprintFavorite>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        // Projected rather than materialised through ToModel: the list screen shows a per-favorite
        // blueprint count, and counting in the query avoids loading every FavoriteBlueprints row to get it.
        return await context.Favorites
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new BlueprintFavorite
            {
                Id = f.Id,
                Name = f.Name,
                BlueprintCount = f.FavoriteBlueprints.Count
            })
            .ToListAsync();
    }

    public async Task<BlueprintFavorite?> GetByNameAsync(string? name)
    {
        if (name == null)
        {
            return new BlueprintFavorite();
        }

        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        Favorite? entity = await context.Favorites.AsNoTracking().FirstOrDefaultAsync(f => f.Name == name);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<BlueprintFavorite> SaveAsync(BlueprintFavorite favorite, List<BlueprintQuantity> quantities)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Favorite? entity = favorite.Id > 0
            ? await context.Favorites.FirstOrDefaultAsync(f => f.Id == favorite.Id)
            : await context.Favorites.FirstOrDefaultAsync(f => f.Name == favorite.Name);

        if (entity == null)
        {
            entity = new Favorite();
            context.Favorites.Add(entity);
        }

        entity.Name = favorite.Name ?? "";

        await context.SaveChangesAsync();

        // Replace the saved quantities wholesale rather than diffing them.
        await context.FavoriteBlueprints.Where(fr => fr.FavoriteId == entity.Id).ExecuteDeleteAsync();

        foreach (BlueprintQuantity blueprintQuantity in quantities)
        {
            context.FavoriteBlueprints.Add(new FavoriteBlueprint
            {
                FavoriteId = entity.Id,
                BlueprintId = blueprintQuantity.Blueprint.Id,
                Quantity = blueprintQuantity.Quantity
            });
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task RenameAsync(int id, string name)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Favorites
            .Where(f => f.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(f => f.Name, name));
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Favorites.Where(f => f.Id == id).ExecuteDeleteAsync();
    }

    public async Task<List<BlueprintQuantity>> GetBlueprintQuantitiesAsync(int favoriteId)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<FavoriteBlueprint> rows = await context.FavoriteBlueprints
            .AsNoTracking()
            .Where(fr => fr.FavoriteId == favoriteId)
            .ToListAsync();

        List<BlueprintQuantity> result = [];
        foreach (FavoriteBlueprint row in rows)
        {
            BlueprintModel? blueprint = await blueprintDAO.GetByIdAsync(row.BlueprintId);
            if (blueprint != null)
            {
                // Id is hardcoded to 0 here to match the old BlueprintFavoriteService quirk: a
                // BlueprintQuantity reconstructed from a favorite never carries the FavoriteBlueprint row's
                // own Id.
                result.Add(new BlueprintQuantity(blueprint, row.Quantity, 0));
            }
        }

        return result;
    }

    private static BlueprintFavorite ToModel(Favorite entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name
    };
}
