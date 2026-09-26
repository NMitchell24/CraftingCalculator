using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryModel>> GetCategoriesAsync();

    Task<CategoryModel?> GetCategoryByIdAsync(int id);

    Task<int> CountCategoriesAsync();

    /// <summary>
    /// Saves or adds the category. If <see cref="CategoryModel.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveCategoryAsync(CategoryModel? category);

    /// <summary>Deletes every category in <paramref name="ids"/>.</summary>
    Task DeleteCategoriesAsync(IEnumerable<int> ids);

    /// <summary>Deletes every category in the selected dataset.</summary>
    Task DeleteAllCategoriesAsync();
}
