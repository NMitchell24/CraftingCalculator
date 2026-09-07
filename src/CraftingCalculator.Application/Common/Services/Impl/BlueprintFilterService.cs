using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class BlueprintFilterService(IBlueprintFilterDAO dao) : IBlueprintFilterService
{
    public Task<List<BlueprintFilter>> GetBlueprintFiltersAsync() => dao.GetAllAsync();

    public Task<BlueprintFilter?> GetBlueprintFilterByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveBlueprintFilterAsync(BlueprintFilter? filter)
        => filter != null ? dao.SaveAsync(filter) : Task.CompletedTask;

    public Task DeleteBlueprintFilterAsync(BlueprintFilter? filter)
        => filter != null ? dao.DeleteAsync(filter.Id) : Task.CompletedTask;
}
