using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class ComponentDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IComponentDAO
{
    public async Task<List<ComponentModel>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<Component> entities = await context.Components
            .AsNoTracking()
            .OrderBy(componentEntity => componentEntity.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<ComponentModel?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        Component? entity = await context.Components.AsNoTracking()
            .FirstOrDefaultAsync(componentEntity => componentEntity.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<ComponentModel> SaveAsync(ComponentModel component)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        Component entity = component.Id > 0
            ? await context.Components.FirstAsync(componentEntity => componentEntity.Id == component.Id)
            : new Component();

        entity.Name = component.Name ?? "";
        entity.Description = component.Description ?? "";
        entity.Cost = component.Cost;
        entity.ProductionTime = component.ProductionTime;

        if (entity.Id == 0)
        {
            context.Components.Add(entity);
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Components.Where(componentEntity => componentEntity.Id == id).ExecuteDeleteAsync();
    }

    private static ComponentModel ToModel(Component entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost,
        ProductionTime = entity.ProductionTime
    };
}
