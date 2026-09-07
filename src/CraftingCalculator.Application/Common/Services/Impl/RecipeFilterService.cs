using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class RecipeFilterService(IRecipeFilterDAO dao) : IRecipeFilterService
{
    public Task<List<RecipeFilter>> GetRecipeFiltersAsync() => dao.GetAllAsync();

    public Task<RecipeFilter?> GetRecipeFilterByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task SaveRecipeFilterAsync(RecipeFilter? filter)
        => filter != null ? dao.SaveAsync(filter) : Task.CompletedTask;

    public Task DeleteRecipeFilterAsync(RecipeFilter? filter)
        => filter != null ? dao.DeleteAsync(filter.Id) : Task.CompletedTask;
}
