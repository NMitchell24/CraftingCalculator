using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.DAO.Impl;

public class CategoryDAO(DatasetScopedContextFactory contextFactory) : ICategoryDAO
{
    public async Task<List<CategoryModel>> GetAllAsync()
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        List<Category> entities = await context.Categories
            .AsNoTracking()
            .OrderBy(categoryEntity => categoryEntity.Name)
            .ToListAsync();

        return [.. entities.Select(ToModel)];
    }

    public async Task<CategoryModel?> GetByIdAsync(int id)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        Category? entity = await context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(categoryEntity => categoryEntity.Id == id);

        return entity != null ? ToModel(entity) : null;
    }

    public async Task<CategoryModel> SaveAsync(CategoryModel category)
    {
        await using CraftingDataContext context = await contextFactory.CreateAsync();

        Category entity = category.Id > 0
            ? await context.Categories.FirstAsync(categoryEntity => categoryEntity.Id == category.Id)
            : new Category();

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
        await using CraftingDataContext context = await contextFactory.CreateAsync();
        await context.Categories.Where(categoryEntity => categoryEntity.Id == id).ExecuteDeleteAsync();
    }

    /// <summary>The model for a loaded category entity. Shared with the DAOs that resolve a
    /// record's category as part of a larger load.</summary>
    internal static CategoryModel ToModel(Category entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description
    };
}
