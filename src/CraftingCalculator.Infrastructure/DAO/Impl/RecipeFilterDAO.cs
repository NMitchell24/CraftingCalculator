using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using RecipeFilterEntity = CraftingCalculator.Domain.Entities.RecipeFilter;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class RecipeFilterDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IRecipeFilterDAO
{
    public async Task<List<RecipeFilter>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<RecipeFilterEntity> entities = await context.RecipeFilters
            .AsNoTracking()
            .OrderByDescending(f => f.Name == RecipeFilter.ALL)
            .ThenBy(f => f.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<RecipeFilter?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        RecipeFilterEntity? entity = await context.RecipeFilters.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<RecipeFilter> SaveAsync(RecipeFilter filter)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        RecipeFilterEntity entity = filter.Id > 0
            ? await context.RecipeFilters.FirstAsync(f => f.Id == filter.Id)
            : new RecipeFilterEntity();

        entity.Name = filter.Name ?? "";
        entity.Description = filter.Description ?? "";

        if (entity.Id == 0)
        {
            context.RecipeFilters.Add(entity);
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.RecipeFilters.Where(f => f.Id == id).ExecuteDeleteAsync();
    }

    private static RecipeFilter ToModel(RecipeFilterEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };
}
