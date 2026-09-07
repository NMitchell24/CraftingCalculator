using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface ICategoryDAO
{
    /// <summary>
    /// Returns all categorys with the <see cref="Category.ALL"/> category first, then alphabetically.
    /// </summary>
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the category if <see cref="Category.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved category with its assigned <see cref="Category.Id"/>.
    /// </summary>
    Task<Category> SaveAsync(Category category);

    /// <summary>
    /// Deletes the category. Blueprints referencing it have their category reference cleared by the
    /// database's SetNull cascade rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);
}
