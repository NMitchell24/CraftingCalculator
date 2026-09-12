using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class ComponentDAO(DatasetScopedContextFactory contextFactory) : IComponentDAO
{
    public async Task<List<ComponentModel>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        List<Component> entities = await context.Components
            .AsNoTracking()
            .Include(componentEntity => componentEntity.Category)
            .OrderBy(componentEntity => componentEntity.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<ComponentModel?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        Component? entity = await context.Components.AsNoTracking()
            .Include(componentEntity => componentEntity.Category)
            .FirstOrDefaultAsync(componentEntity => componentEntity.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<ComponentModel> SaveAsync(ComponentModel component)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        Component entity = component.Id > 0
            ? await context.Components.FirstAsync(componentEntity => componentEntity.Id == component.Id)
            : new Component();

        entity.Name = component.Name ?? "";
        entity.Description = component.Description ?? "";
        entity.Cost = component.Cost;
        entity.ProductionTime = component.ProductionTime;
        entity.CategoryId = component.Category?.Id;

        if (entity.Id == 0)
        {
            context.Components.Add(entity);
        }

        await context.SaveChangesAsync();

        // The caller's model rather than ToModel(entity), which would report a null Category: only
        // CategoryId was set above, and loading the navigation back would cost a query for a value the
        // caller already holds. Same shape as BlueprintDAO.SaveAsync.
        component.Id = entity.Id;

        return component;
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        await context.Components.Where(componentEntity => componentEntity.Id == id).ExecuteDeleteAsync();
    }

    private static ComponentModel ToModel(Component entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        Category = entity.Category is { } categoryEntity ? CategoryDAO.ToModel(categoryEntity) : null,
        Cost = entity.Cost,
        ProductionTime = entity.ProductionTime
    };
}
