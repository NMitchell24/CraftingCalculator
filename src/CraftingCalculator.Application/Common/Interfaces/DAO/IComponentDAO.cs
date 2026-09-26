using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IComponentDAO
{
    Task<List<ComponentModel>> GetAllAsync();

    Task<ComponentModel?> GetByIdAsync(int id);

    /// <summary>Returns how many components there are.</summary>
    Task<int> CountAsync();

    /// <summary>
    /// Adds the component if <see cref="ComponentModel.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved component with its assigned <see cref="ComponentModel.Id"/>.
    /// </summary>
    Task<ComponentModel> SaveAsync(ComponentModel component);

    /// <summary>
    /// Deletes every component in <paramref name="ids"/>. Blueprint parts that name them are removed by the
    /// database's cascade delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(IEnumerable<int> ids);

    /// <summary>Deletes every component in the selected dataset, the same way as <see cref="DeleteAsync"/>.</summary>
    Task DeleteAllAsync();
}
