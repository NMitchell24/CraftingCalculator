using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IBlueprintFilterDAO
{
    /// <summary>
    /// Returns all filters with the <see cref="BlueprintFilter.ALL"/> filter first, then alphabetically.
    /// </summary>
    Task<List<BlueprintFilter>> GetAllAsync();

    Task<BlueprintFilter?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the filter if <see cref="BlueprintFilter.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved filter with its assigned <see cref="BlueprintFilter.Id"/>.
    /// </summary>
    Task<BlueprintFilter> SaveAsync(BlueprintFilter filter);

    /// <summary>
    /// Deletes the filter. Blueprints referencing it have their filter reference cleared by the
    /// database's SetNull cascade rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);
}
