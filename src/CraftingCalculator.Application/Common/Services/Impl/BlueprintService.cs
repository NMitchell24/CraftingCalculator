using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class BlueprintService(IBlueprintDAO dao) : IBlueprintService
{
    public Task<List<BlueprintModel>> GetBlueprintsByCategoryAsync(CategoryModel category) => dao.GetByCategoryAsync(category);

    public Task<BlueprintModel?> GetBlueprintByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<List<BlueprintModel>> GetAllBlueprintsAsync() => dao.GetAllAsync();

    public Task SaveBlueprintAsync(BlueprintModel? blueprint) => blueprint != null ? dao.SaveAsync(blueprint) : Task.CompletedTask;

    public Task DeleteBlueprintAsync(BlueprintModel? blueprint) => blueprint != null ? dao.DeleteAsync(blueprint.Id) : Task.CompletedTask;

    public BlueprintNode GetBlueprintNode(BlueprintModel blueprint, long quantity) => BlueprintProcessor.BuildNode(blueprint, quantity);
}
