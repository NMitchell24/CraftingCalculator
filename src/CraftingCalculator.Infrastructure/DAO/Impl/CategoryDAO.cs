using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using CategoryEntity = CraftingCalculator.Domain.Entities.Category;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class CategoryDAO(IDbContextFactory<CraftingDataContext> contextFactory) : ICategoryDAO
{
    public async Task<List<Category>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        List<CategoryEntity> entities = await context.Categories
            .AsNoTracking()
            .OrderByDescending(f => f.Name == Category.ALL)
            .ThenBy(f => f.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        CategoryEntity? entity = await context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<Category> SaveAsync(Category category)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();

        CategoryEntity entity = category.Id > 0
            ? await context.Categories.FirstAsync(f => f.Id == category.Id)
            : new CategoryEntity();

        entity.Name = category.Name ?? "";
        entity.Description = category.Description ?? "";

        if (entity.Id == 0)
        {
            context.Categories.Add(entity);
        }

        await context.SaveChangesAsync();

        return ToModel(entity);
    }

    public async Task DeleteAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateDbContextAsync();
        await context.Categories.Where(f => f.Id == id).ExecuteDeleteAsync();
    }

    private static Category ToModel(CategoryEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };
}
