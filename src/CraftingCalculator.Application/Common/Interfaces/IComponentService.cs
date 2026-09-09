using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IComponentService
{
    Task<List<ComponentModel>> GetAllComponentsAsync();

    Task<ComponentModel?> GetComponentByIdAsync(int id);

    /// <summary>
    /// Saves or adds the component. If <see cref="ComponentModel.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveComponentAsync(ComponentModel? component);

    Task DeleteComponentAsync(ComponentModel? component);
}
