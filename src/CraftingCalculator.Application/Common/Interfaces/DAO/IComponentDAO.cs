using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IComponentDAO
{
    Task<List<ComponentModel>> GetAllAsync();

    Task<ComponentModel?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the component if <see cref="ComponentModel.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved component with its assigned <see cref="ComponentModel.Id"/>.
    /// </summary>
    Task<ComponentModel> SaveAsync(ComponentModel component);

    Task DeleteAsync(int id);
}
