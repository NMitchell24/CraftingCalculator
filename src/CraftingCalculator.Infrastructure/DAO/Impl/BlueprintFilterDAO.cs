using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using BlueprintFilterEntity = CraftingCalculator.Domain.Entities.BlueprintFilter;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class BlueprintFilterDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IBlueprintFilterDAO
{
    public async Task<List<BlueprintFilter>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<BlueprintFilterEntity> entities = await context.BlueprintFilters
            .AsNoTracking()
            .OrderByDescending(f => f.Name == BlueprintFilter.ALL)
            .ThenBy(f => f.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<BlueprintFilter?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        BlueprintFilterEntity? entity = await context.BlueprintFilters.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<BlueprintFilter> SaveAsync(BlueprintFilter filter)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        BlueprintFilterEntity entity = filter.Id > 0
            ? await context.BlueprintFilters.FirstAsync(f => f.Id == filter.Id)
            : new BlueprintFilterEntity();

        entity.Name = filter.Name ?? "";
        entity.Description = filter.Description ?? "";

        if (entity.Id == 0)
        {
            context.BlueprintFilters.Add(entity);
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.BlueprintFilters.Where(f => f.Id == id).ExecuteDeleteAsync();
    }

    private static BlueprintFilter ToModel(BlueprintFilterEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };
}
