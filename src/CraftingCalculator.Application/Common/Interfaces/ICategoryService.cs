using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryModel>> GetCategoriesAsync();

    Task<CategoryModel?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Saves or adds the category. If <see cref="CategoryModel.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveCategoryAsync(CategoryModel? category);

    Task DeleteCategoryAsync(CategoryModel? category);
}
