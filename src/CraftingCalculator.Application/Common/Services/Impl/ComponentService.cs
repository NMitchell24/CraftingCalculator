using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class ComponentService(IComponentDAO dao) : IComponentService
{
    public Task<List<ComponentModel>> GetAllComponentsAsync() => dao.GetAllAsync();

    public Task<ComponentModel?> GetComponentByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<int> CountComponentsAsync() => dao.CountAsync();

    public Task SaveComponentAsync(ComponentModel? component)
        => component != null ? dao.SaveAsync(component) : Task.CompletedTask;

    public Task DeleteComponentsAsync(IEnumerable<int> ids) => dao.DeleteAsync(ids);

    public Task DeleteAllComponentsAsync() => dao.DeleteAllAsync();
}
