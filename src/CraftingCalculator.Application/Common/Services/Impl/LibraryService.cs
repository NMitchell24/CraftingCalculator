using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class LibraryService(
    IComponentService componentService,
    IRecipeFilterService recipeFilterService,
    IRecipeService recipeService) : ILibraryService
{
    public async Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type) => type switch
    {
        DataType.Component => [.. await componentService.GetAllComponentsAsync()],
        DataType.RecipeFilter => [.. (await recipeFilterService.GetRecipeFiltersAsync())
            // Excluded by id rather than by name: a user is free to create a category of their own
            // called "All", and matching on the name would hide it here permanently - leaving it
            // impossible to rename or delete.
            .Where(f => f.Id != DatabaseSeedConstants.AllFilterId)],
        DataType.Recipe => [.. await recipeService.GetAllRecipesAsync()],
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public async Task<IBaseDataRecord?> GetRecordAsync(DataType type, int id) => type switch
    {
        DataType.Component => await componentService.GetComponentByIdAsync(id),
        DataType.RecipeFilter => await recipeFilterService.GetRecipeFilterByIdAsync(id),
        DataType.Recipe => await recipeService.GetRecipeByIdAsync(id),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public Task SaveRecordAsync(IBaseDataRecord? record) => record switch
    {
        Component component => componentService.SaveComponentAsync(component),
        RecipeFilter filter => recipeFilterService.SaveRecipeFilterAsync(filter),
        Recipe recipe => recipeService.SaveRecipeAsync(recipe),
        _ => Task.CompletedTask
    };

    public Task DeleteRecordAsync(IBaseDataRecord? record) => record switch
    {
        Component component => componentService.DeleteComponentAsync(component),
        RecipeFilter filter => recipeFilterService.DeleteRecipeFilterAsync(filter),
        Recipe recipe => recipeService.DeleteRecipeAsync(recipe),
        _ => Task.CompletedTask
    };
}
