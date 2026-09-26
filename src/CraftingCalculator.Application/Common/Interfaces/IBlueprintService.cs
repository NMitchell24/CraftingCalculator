using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IBlueprintService
{
    Task<BlueprintModel?> GetBlueprintByIdAsync(int id);

    /// <summary>
    /// The blueprints with the given <paramref name="ids"/>, by id, each built out in full. An id with no blueprint
    /// has no entry.
    /// </summary>
    Task<Dictionary<int, BlueprintModel>> GetBlueprintsByIdsAsync(IReadOnlyCollection<int> ids);

    /// <summary>Every blueprint without its parts, ordered by name.</summary>
    Task<List<BlueprintSummary>> GetBlueprintSummariesAsync();

    /// <summary>
    /// Every blueprint that can be nested inside the blueprint with <paramref name="blueprintId"/> without closing a
    /// loop, without its parts, ordered by name: all of them except that blueprint and every blueprint that already
    /// nests it at any depth. An unsaved blueprint's id is 0, which leaves every blueprint in.
    /// </summary>
    Task<List<BlueprintSummary>> GetNestableBlueprintSummariesAsync(int blueprintId);

    Task<int> CountBlueprintsAsync();

    /// <summary>
    /// Saves or adds the blueprint. If <see cref="BlueprintModel.Id"/> is 0 a new one is added, otherwise the
    /// existing record (including its components) is updated.
    /// </summary>
    Task SaveBlueprintAsync(BlueprintModel? blueprint);

    /// <summary>Deletes every blueprint in <paramref name="ids"/>.</summary>
    Task DeleteBlueprintsAsync(IEnumerable<int> ids);

    /// <summary>Deletes every blueprint in the selected dataset.</summary>
    Task DeleteAllBlueprintsAsync();
}
