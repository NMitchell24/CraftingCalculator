using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IBlueprintService
{
    Task<List<BlueprintModel>> GetBlueprintsByCategoryAsync(CategoryModel category);

    Task<BlueprintModel?> GetBlueprintByIdAsync(int id);

    Task<List<BlueprintModel>> GetAllBlueprintsAsync();

    /// <summary>
    /// Saves or adds the blueprint. If <see cref="BlueprintModel.Id"/> is 0 a new one is added, otherwise the
    /// existing record (including its components) is updated.
    /// </summary>
    Task SaveBlueprintAsync(BlueprintModel? blueprint);

    Task DeleteBlueprintAsync(BlueprintModel? blueprint);

    /// <summary>
    /// Builds the blueprint's component breakdown as a <see cref="BlueprintNode"/> tree, scaled by
    /// <paramref name="quantity"/>.
    /// </summary>
    BlueprintNode GetBlueprintNode(BlueprintModel blueprint, long quantity);
}
