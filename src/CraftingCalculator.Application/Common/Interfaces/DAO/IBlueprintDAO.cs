using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IBlueprintDAO
{
    /// <summary>
    /// Returns the blueprint with its full component graph populated (components and, recursively,
    /// child blueprints), or null if no blueprint with this id exists.
    /// </summary>
    Task<BlueprintModel?> GetByIdAsync(int id);

    /// <summary>
    /// Returns the blueprints with the given <paramref name="ids"/>, by id, each with its full component graph
    /// populated. An id with no blueprint has no entry.
    /// </summary>
    Task<Dictionary<int, BlueprintModel>> GetByIdsAsync(IReadOnlyCollection<int> ids);

    /// <summary>Returns every blueprint without its parts, ordered by name.</summary>
    Task<List<BlueprintSummary>> GetSummariesAsync();

    /// <summary>
    /// Returns the ids of every blueprint that nests the blueprint with <paramref name="id"/> at any depth, not counting
    /// that blueprint itself. Empty when nothing nests it or no blueprint has the id.
    /// </summary>
    Task<HashSet<int>> GetAncestorIdsAsync(int id);

    /// <summary>Returns how many blueprints there are.</summary>
    Task<int> CountAsync();

    /// <summary>
    /// Adds the blueprint if <see cref="BlueprintModel.Id"/> is 0, otherwise updates the existing record
    /// (including its component and child-blueprint components). Returns the saved blueprint with its
    /// assigned <see cref="BlueprintModel.Id"/>.
    /// </summary>
    Task<BlueprintModel> SaveAsync(BlueprintModel blueprint);

    /// <summary>
    /// Deletes every blueprint in <paramref name="ids"/>. Their parts, their places as a part of other blueprints,
    /// and any favorite entries referencing them are removed by the database's cascade delete rather than by the
    /// caller.
    /// </summary>
    Task DeleteAsync(IEnumerable<int> ids);

    /// <summary>Deletes every blueprint in the selected dataset, the same way as <see cref="DeleteAsync"/>.</summary>
    Task DeleteAllAsync();
}
