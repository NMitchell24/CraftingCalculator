using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class ComponentService(IComponentDAO dao) : IComponentService
{
    public Task<List<Component>> GetAllComponentsAsync() => dao.GetAllAsync();

    public Task<Component?> GetComponentByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveComponentAsync(Component? component)
        => component != null ? dao.SaveAsync(component) : Task.CompletedTask;

    public Task DeleteComponentAsync(Component? component)
        => component != null ? dao.DeleteAsync(component.Id) : Task.CompletedTask;
}
