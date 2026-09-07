using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IBlueprintDAO
{
    /// <summary>
    /// Returns every blueprint matching <paramref name="filter"/>, or all blueprints when
    /// <paramref name="filter"/> is the <see cref="BlueprintFilter.ALL"/> filter. Each blueprint is
    /// returned with its <see cref="Blueprint.Components"/> and <see cref="Blueprint.ChildBlueprints"/>
    /// populated.
    /// </summary>
    Task<List<Blueprint>> GetByFilterAsync(BlueprintFilter filter);

    /// <summary>
    /// Returns the blueprint with its full component graph populated (components and, recursively,
    /// child blueprints), or null if no blueprint with this id exists.
    /// </summary>
    Task<Blueprint?> GetByIdAsync(int id);

    /// <summary>
    /// Returns every blueprint, each with its full component graph populated.
    /// </summary>
    Task<List<Blueprint>> GetAllAsync();

    /// <summary>
    /// Adds the blueprint if <see cref="Blueprint.Id"/> is 0, otherwise updates the existing record
    /// (including its component and child-blueprint components). Returns the saved blueprint with its
    /// assigned <see cref="Blueprint.Id"/>.
    /// </summary>
    Task<Blueprint> SaveAsync(Blueprint blueprint);

    /// <summary>
    /// Deletes the blueprint. Its components, any blueprints that use it as a child, and any favorite
    /// entries referencing it are removed by the database's cascade delete rather than by the
    /// caller.
    /// </summary>
    Task DeleteAsync(int id);
}
