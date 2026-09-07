using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// The Dataset screen's view of the three user-editable record types, addressed uniformly by
/// <see cref="DataType"/> so callers do not each have to branch on it.
/// </summary>
public interface IDatasetService
{
    /// <summary>
    /// Every record of <paramref name="type"/>, ordered by name.
    /// <see cref="DataType.Category"/> excludes the seeded <see cref="Category.ALL"/> row,
    /// which is a category sentinel rather than a category a user can edit.
    /// </summary>
    Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type);

    /// <summary>Returns the record, or null when no record of that type has that id.</summary>
    Task<IBaseDataRecord?> GetRecordAsync(DataType type, int id);

    /// <summary>
    /// Saves or adds the record. If <see cref="IBaseDataRecord.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveRecordAsync(IBaseDataRecord? record);

    Task DeleteRecordAsync(IBaseDataRecord? record);
}
