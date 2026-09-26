using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class RecordService(
    IComponentService componentService,
    ICategoryService categoryService,
    IBlueprintService blueprintService) : IRecordService
{
    public async Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type) => type switch
    {
        DataType.Component => [.. await componentService.GetAllComponentsAsync()],
        DataType.Category => [.. await categoryService.GetCategoriesAsync()],
        DataType.Blueprint => [.. await blueprintService.GetBlueprintSummariesAsync()],
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public Task<int> CountRecordsAsync(DataType type) => type switch
    {
        DataType.Component => componentService.CountComponentsAsync(),
        DataType.Category => categoryService.CountCategoriesAsync(),
        DataType.Blueprint => blueprintService.CountBlueprintsAsync(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public async Task<IBaseDataRecord?> GetRecordAsync(DataType type, int id) => type switch
    {
        DataType.Component => await componentService.GetComponentByIdAsync(id),
        DataType.Category => await categoryService.GetCategoryByIdAsync(id),
        DataType.Blueprint => await blueprintService.GetBlueprintByIdAsync(id),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public async Task<IBaseDataRecord?> GetCopyAsync(DataType type, int id)
    {
        // The record is read fresh for this call and nothing else holds it, so it becomes the copy in place.
        // Its parts keep their ids: a copied blueprint uses the same components and blueprints as the original.
        if (await GetRecordAsync(type, id) is not { } copy)
        {
            return null;
        }

        copy.Id = 0;
        copy.Name += DatasetConstants.CopySuffix;

        return copy;
    }

    public Task SaveRecordAsync(IBaseDataRecord? record) => record switch
    {
        ComponentModel component => componentService.SaveComponentAsync(component),
        CategoryModel category => categoryService.SaveCategoryAsync(category),
        BlueprintModel blueprint => blueprintService.SaveBlueprintAsync(blueprint),
        _ => Task.CompletedTask
    };

    public Task DeleteRecordAsync(IBaseDataRecord? record) =>
        record is null ? Task.CompletedTask : DeleteRecordsAsync(record.Type, [record.Id]);

    public Task DeleteRecordsAsync(DataType type, IEnumerable<int> ids) => type switch
    {
        DataType.Component => componentService.DeleteComponentsAsync(ids),
        DataType.Category => categoryService.DeleteCategoriesAsync(ids),
        DataType.Blueprint => blueprintService.DeleteBlueprintsAsync(ids),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public Task DeleteAllOfTypeAsync(DataType type) => type switch
    {
        DataType.Component => componentService.DeleteAllComponentsAsync(),
        DataType.Category => categoryService.DeleteAllCategoriesAsync(),
        DataType.Blueprint => blueprintService.DeleteAllBlueprintsAsync(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}
