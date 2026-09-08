using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class BlueprintService(IBlueprintDAO dao) : IBlueprintService
{
    public Task<List<Blueprint>> GetBlueprintsByCategoryAsync(Category category) => dao.GetByCategoryAsync(category);

    public Task<Blueprint?> GetBlueprintByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<List<Blueprint>> GetAllBlueprintsAsync() => dao.GetAllAsync();

    public Task SaveBlueprintAsync(Blueprint? blueprint) => blueprint != null ? dao.SaveAsync(blueprint) : Task.CompletedTask;

    public Task DeleteBlueprintAsync(Blueprint? blueprint) => blueprint != null ? dao.DeleteAsync(blueprint.Id) : Task.CompletedTask;

    public (ComponentMap Components, BlueprintMap Surplus) GetFlattenedComponents(Blueprint blueprint, long quantity) =>
        BlueprintProcessor.Flatten(blueprint, quantity);

    public BlueprintNode GetBlueprintNode(Blueprint blueprint, long quantity) => BlueprintProcessor.BuildNode(blueprint, quantity);
}
