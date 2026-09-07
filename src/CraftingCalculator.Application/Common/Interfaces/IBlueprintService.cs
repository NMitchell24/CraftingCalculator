using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IBlueprintService
{
    Task<List<Blueprint>> GetBlueprintsByCategoryAsync(Category category);

    Task<Blueprint?> GetBlueprintByIdAsync(int id);

    Task<List<Blueprint>> GetAllBlueprintsAsync();

    /// <summary>
    /// Saves or adds the blueprint. If <see cref="Blueprint.Id"/> is 0 a new one is added, otherwise the
    /// existing record (including its components) is updated.
    /// </summary>
    Task SaveBlueprintAsync(Blueprint? blueprint);

    Task DeleteBlueprintAsync(Blueprint? blueprint);

    /// <summary>
    /// Flattens the blueprint's own components and every (recursively) nested child blueprint's
    /// components into one combined <see cref="ComponentMap"/>.
    /// </summary>
    ComponentMap GetFlattenedComponents(Blueprint blueprint);

    /// <summary>
    /// Builds the blueprint's component breakdown as a <see cref="BlueprintNode"/> tree, scaled by
    /// <paramref name="quantity"/>.
    /// </summary>
    BlueprintNode GetBlueprintNode(Blueprint blueprint, long quantity);
}
