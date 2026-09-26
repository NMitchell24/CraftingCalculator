using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class CategoryService(ICategoryDAO dao) : ICategoryService
{
    public Task<List<CategoryModel>> GetCategoriesAsync() => dao.GetAllAsync();

    public Task<CategoryModel?> GetCategoryByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<int> CountCategoriesAsync() => dao.CountAsync();

    public Task SaveCategoryAsync(CategoryModel? category)
        => category != null ? dao.SaveAsync(category) : Task.CompletedTask;

    public Task DeleteCategoriesAsync(IEnumerable<int> ids) => dao.DeleteAsync(ids);

    public Task DeleteAllCategoriesAsync() => dao.DeleteAllAsync();
}
