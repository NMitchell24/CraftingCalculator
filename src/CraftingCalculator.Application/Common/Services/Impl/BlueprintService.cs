using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class BlueprintService(IBlueprintDAO dao) : IBlueprintService
{
    public Task<BlueprintModel?> GetBlueprintByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<Dictionary<int, BlueprintModel>> GetBlueprintsByIdsAsync(IReadOnlyCollection<int> ids) => dao.GetByIdsAsync(ids);

    public Task<List<BlueprintSummary>> GetBlueprintSummariesAsync() => dao.GetSummariesAsync();

    public async Task<List<BlueprintSummary>> GetNestableBlueprintSummariesAsync(int blueprintId)
    {
        // A loop can only be closed by nesting the blueprint in itself or in something it already sits under.
        HashSet<int> ancestorIds = await dao.GetAncestorIdsAsync(blueprintId);
        List<BlueprintSummary> summaries = await dao.GetSummariesAsync();

        return [.. summaries.Where(summary => summary.Id != blueprintId && !ancestorIds.Contains(summary.Id))];
    }

    public Task<int> CountBlueprintsAsync() => dao.CountAsync();

    public Task SaveBlueprintAsync(BlueprintModel? blueprint) => blueprint != null ? dao.SaveAsync(blueprint) : Task.CompletedTask;

    public Task DeleteBlueprintAsync(int id) => dao.DeleteAsync(id);

    public BlueprintNode GetBlueprintNode(BlueprintModel blueprint, long quantity, Datasettings settings) =>
        BlueprintProcessor.BuildNode(blueprint, quantity, settings);
}
