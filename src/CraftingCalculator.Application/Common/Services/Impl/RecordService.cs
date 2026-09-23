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

    public Task DeleteRecordAsync(IBaseDataRecord? record) => record switch
    {
        ComponentModel component => componentService.DeleteComponentAsync(component),
        CategoryModel category => categoryService.DeleteCategoryAsync(category),
        // By type rather than by class: a list hands over a BlueprintSummary, the editor a BlueprintModel.
        { Type: DataType.Blueprint, Id: var id } => blueprintService.DeleteBlueprintAsync(id),
        _ => Task.CompletedTask
    };

    public async Task DeleteRecordsAsync(IEnumerable<IBaseDataRecord> records)
    {
        // Sequential rather than Task.WhenAll: the per-type services share one DbContext factory and a
        // delete cascades, so overlapping deletes would race each other's cascade.
        foreach (IBaseDataRecord record in records)
        {
            await DeleteRecordAsync(record);
        }
    }

    public async Task DeleteAllOfTypeAsync(DataType type) =>
        await DeleteRecordsAsync(await GetRecordsAsync(type));
}
