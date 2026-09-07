using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<List<Category>> GetCategoriesAsync();

    Task<Category?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Saves or adds the category. If <see cref="Category.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveCategoryAsync(Category? category);

    Task DeleteCategoryAsync(Category? category);
}
