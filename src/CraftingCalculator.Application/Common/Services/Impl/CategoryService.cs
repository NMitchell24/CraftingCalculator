using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class CategoryService(ICategoryDAO dao) : ICategoryService
{
    public Task<List<Category>> GetCategoriesAsync() => dao.GetAllAsync();

    public Task<Category?> GetCategoryByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveCategoryAsync(Category? category)
        => category != null ? dao.SaveAsync(category) : Task.CompletedTask;

    public Task DeleteCategoryAsync(Category? category)
        => category != null ? dao.DeleteAsync(category.Id) : Task.CompletedTask;
}
