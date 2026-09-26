using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface ICategoryDAO
{
    /// <summary>Returns all categories, ordered alphabetically by name.</summary>
    Task<List<CategoryModel>> GetAllAsync();

    Task<CategoryModel?> GetByIdAsync(int id);

    /// <summary>Returns how many categories there are.</summary>
    Task<int> CountAsync();

    /// <summary>
    /// Adds the category if <see cref="CategoryModel.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved category with its assigned <see cref="CategoryModel.Id"/>.
    /// </summary>
    Task<CategoryModel> SaveAsync(CategoryModel category);

    /// <summary>
    /// Deletes every category in <paramref name="ids"/>. Records referencing them have their category reference
    /// cleared by the database's SetNull cascade rather than by the caller.
    /// </summary>
    Task DeleteAsync(IEnumerable<int> ids);

    /// <summary>Deletes every category in the selected dataset, the same way as <see cref="DeleteAsync"/>.</summary>
    Task DeleteAllAsync();
}
