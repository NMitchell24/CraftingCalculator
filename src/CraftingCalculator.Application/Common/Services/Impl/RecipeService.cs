using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class RecipeService(IRecipeDAO dao) : IRecipeService
{
    public Task<List<Recipe>> GetRecipesByFilterAsync(RecipeFilter filter) => dao.GetByFilterAsync(filter);

    public Task<Recipe?> GetRecipeByIdAsync(int id) => dao.GetByIdAsync(id);

    public Task<List<Recipe>> GetAllRecipesAsync() => dao.GetAllAsync();

    public Task SaveRecipeAsync(Recipe? recipe) => recipe != null ? dao.SaveAsync(recipe) : Task.CompletedTask;

    public Task DeleteRecipeAsync(Recipe? recipe) => recipe != null ? dao.DeleteAsync(recipe.Id) : Task.CompletedTask;

    public ComponentMap GetFlattenedComponents(Recipe recipe) => RecipeProcessor.Flatten(recipe);

    public RecipeNode GetRecipeNode(Recipe recipe, long quantity) => RecipeProcessor.BuildNode(recipe, quantity);
}
