using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class DatasetService(
    IComponentService componentService,
    ICategoryService categoryService,
    IBlueprintService blueprintService) : IDatasetService
{
    public async Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type) => type switch
    {
        DataType.Component => [.. await componentService.GetAllComponentsAsync()],
        DataType.Category => [.. (await categoryService.GetCategoriesAsync())
            // Excluded by id rather than by name: a user is free to create a category of their own
            // called "All", and matching on the name would hide it here permanently - leaving it
            // impossible to rename or delete.
            .Where(category => category.Id != DatabaseSeedConstants.AllCategoryId)],
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
        ComponentModel component => componentService.SaveComponentAsync(component),
        CategoryModel category => categoryService.SaveCategoryAsync(category),
        BlueprintModel blueprint => blueprintService.SaveBlueprintAsync(blueprint),
        _ => Task.CompletedTask
    };

    public Task DeleteRecordAsync(IBaseDataRecord? record) => record switch
    {
        ComponentModel component => componentService.DeleteComponentAsync(component),
        CategoryModel category => categoryService.DeleteCategoryAsync(category),
        BlueprintModel blueprint => blueprintService.DeleteBlueprintAsync(blueprint),
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

    // Goes through GetRecordsAsync rather than the per-type service so the Category sentinel stays
    // excluded here for the same reason it is excluded there.
    public async Task DeleteAllOfTypeAsync(DataType type) =>
        await DeleteRecordsAsync(await GetRecordsAsync(type));
}
