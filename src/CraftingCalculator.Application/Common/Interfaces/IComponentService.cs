using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

public interface IComponentService
{
    Task<List<ComponentModel>> GetAllComponentsAsync();

    Task<ComponentModel?> GetComponentByIdAsync(int id);

    Task<int> CountComponentsAsync();

    /// <summary>
    /// Saves or adds the component. If <see cref="ComponentModel.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveComponentAsync(ComponentModel? component);

    /// <summary>Deletes every component in <paramref name="ids"/>.</summary>
    Task DeleteComponentsAsync(IEnumerable<int> ids);

    /// <summary>Deletes every component in the selected dataset.</summary>
    Task DeleteAllComponentsAsync();
}
