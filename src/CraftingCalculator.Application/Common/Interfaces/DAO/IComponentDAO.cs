using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IComponentDAO
{
    Task<List<Component>> GetAllAsync();

    Task<Component?> GetByIdAsync(int id);

    /// <summary>
    /// Adds the component if <see cref="Component.Id"/> is 0, otherwise updates the existing record.
    /// Returns the saved component with its assigned <see cref="Component.Id"/>.
    /// </summary>
    Task<Component> SaveAsync(Component component);

    Task DeleteAsync(int id);
}
