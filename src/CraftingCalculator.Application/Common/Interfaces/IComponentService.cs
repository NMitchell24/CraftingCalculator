using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IComponentService
{
    Task<List<Component>> GetAllComponentsAsync();

    Task<Component?> GetComponentByIdAsync(int id);

    /// <summary>
    /// Saves or adds the component. If <see cref="Component.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveComponentAsync(Component? component);

    Task DeleteComponentAsync(Component? component);
}
