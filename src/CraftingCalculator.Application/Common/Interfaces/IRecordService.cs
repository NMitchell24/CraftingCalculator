using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// The Dataset screen's view of the three user-editable record types, addressed uniformly by
/// <see cref="DataType"/> so callers do not each have to branch on it.
/// </summary>
public interface IRecordService
{
    /// <summary>
    /// Every record of <paramref name="type"/>, ordered by name. Blueprints are listed as
    /// <see cref="BlueprintSummary"/>; <see cref="GetRecordAsync"/> returns one with its parts.
    /// </summary>
    Task<List<IBaseDataRecord>> GetRecordsAsync(DataType type);

    /// <summary>How many records of <paramref name="type"/> there are.</summary>
    Task<int> CountRecordsAsync(DataType type);

    /// <summary>Returns the record, or null when no record of that type has that id.</summary>
    Task<IBaseDataRecord?> GetRecordAsync(DataType type, int id);

    /// <summary>
    /// Returns an unsaved duplicate of the record, with the same parts and details, an <see cref="IBaseDataRecord.Id"/>
    /// of 0 and <see cref="DatasetConstants.CopySuffix"/> on its name; or null when no record of that type has that id.
    /// </summary>
    Task<IBaseDataRecord?> GetCopyAsync(DataType type, int id);

    /// <summary>
    /// Saves or adds the record. If <see cref="IBaseDataRecord.Id"/> is 0 a new one is added,
    /// otherwise the existing record is updated.
    /// </summary>
    Task SaveRecordAsync(IBaseDataRecord? record);

    Task DeleteRecordAsync(IBaseDataRecord? record);

    /// <summary>Deletes every record of <paramref name="type"/> whose id is in <paramref name="ids"/>, all or none.</summary>
    Task DeleteRecordsAsync(DataType type, IEnumerable<int> ids);

    /// <summary>Deletes every record of <paramref name="type"/>, all or none.</summary>
    Task DeleteAllOfTypeAsync(DataType type);
}
