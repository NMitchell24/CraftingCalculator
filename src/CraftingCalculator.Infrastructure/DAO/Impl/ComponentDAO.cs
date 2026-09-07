using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using ComponentEntity = CraftingCalculator.Domain.Entities.Component;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class ComponentDAO(IDbContextFactory<CraftingDataContext> contextFactory) : IComponentDAO
{
    public async Task<List<Component>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<ComponentEntity> entities = await context.Components
            .AsNoTracking()
            .OrderBy(componentEntity => componentEntity.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<Component?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        ComponentEntity? entity = await context.Components.AsNoTracking()
            .FirstOrDefaultAsync(componentEntity => componentEntity.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<Component> SaveAsync(Component component)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        ComponentEntity entity = component.Id > 0
            ? await context.Components.FirstAsync(componentEntity => componentEntity.Id == component.Id)
            : new ComponentEntity();

        entity.Name = component.Name ?? "";
        entity.Description = component.Description ?? "";
        entity.Cost = component.Cost;

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

    private static Component ToModel(ComponentEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Cost = entity.Cost
    };
}
