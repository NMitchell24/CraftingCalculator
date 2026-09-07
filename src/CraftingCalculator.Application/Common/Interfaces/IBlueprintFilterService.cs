using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IBlueprintFilterService
{
    Task<List<BlueprintFilter>> GetBlueprintFiltersAsync();

    Task<BlueprintFilter?> GetBlueprintFilterByIdAsync(int id);

    /// <summary>
    /// Saves or adds the filter. If <see cref="BlueprintFilter.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveBlueprintFilterAsync(BlueprintFilter? filter);

    Task DeleteBlueprintFilterAsync(BlueprintFilter? filter);
}
