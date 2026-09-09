using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface ICategoryDAO
{
    /// <summary>
    /// Returns all categories with the <see cref="CategoryModel.All"/> category first, then alphabetically.
    /// </summary>
    Task<List<CategoryModel>> GetAllAsync();

    Task<CategoryModel?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the category if <see cref="CategoryModel.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved category with its assigned <see cref="CategoryModel.Id"/>.
    /// </summary>
    Task<CategoryModel> SaveAsync(CategoryModel category);

    /// <summary>
    /// Deletes the category. Blueprints referencing it have their category reference cleared by the
    /// database's SetNull cascade rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);
}
