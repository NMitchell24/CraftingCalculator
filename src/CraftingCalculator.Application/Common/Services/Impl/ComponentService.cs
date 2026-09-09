using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class ComponentService(IComponentDAO dao) : IComponentService
{
    public Task<List<ComponentModel>> GetAllComponentsAsync() => dao.GetAllAsync();

    public Task<ComponentModel?> GetComponentByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveComponentAsync(ComponentModel? component)
        => component != null ? dao.SaveAsync(component) : Task.CompletedTask;

    public Task DeleteComponentAsync(ComponentModel? component)
        => component != null ? dao.DeleteAsync(component.Id) : Task.CompletedTask;
}
