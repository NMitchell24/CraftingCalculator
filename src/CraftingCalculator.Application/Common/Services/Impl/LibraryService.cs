using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class LibraryService(
    IComponentService componentService,
    ICategoryService categoryService,
    IBlueprintService blueprintService) : ILibraryService
{
    public async Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type) => type switch
    {
        DataType.Component => [.. await componentService.GetAllComponentsAsync()],
        DataType.Category => [.. (await categoryService.GetCategorysAsync())
            // Excluded by id rather than by name: a user is free to create a category of their own
            // called "All", and matching on the name would hide it here permanently - leaving it
            // impossible to rename or delete.
            .Where(f => f.Id != DatabaseSeedConstants.AllCategoryId)],
        DataType.Blueprint => [.. await blueprintService.GetAllBlueprintsAsync()],
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public async Task<IBaseDataRecord?> GetRecordAsync(DataType type, int id) => type switch
    {
        DataType.Component => await componentService.GetComponentByIdAsync(id),
        DataType.Category => await categoryService.GetCategoryByIdAsync(id),
        DataType.Blueprint => await blueprintService.GetBlueprintByIdAsync(id),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public Task SaveRecordAsync(IBaseDataRecord? record) => record switch
    {
        Component component => componentService.SaveComponentAsync(component),
        Category category => categoryService.SaveCategoryAsync(category),
        Blueprint blueprint => blueprintService.SaveBlueprintAsync(blueprint),
        _ => Task.CompletedTask
    };

    public Task DeleteRecordAsync(IBaseDataRecord? record) => record switch
    {
        Component component => componentService.DeleteComponentAsync(component),
        Category category => categoryService.DeleteCategoryAsync(category),
        Blueprint blueprint => blueprintService.DeleteBlueprintAsync(blueprint),
        _ => Task.CompletedTask
    };
}
