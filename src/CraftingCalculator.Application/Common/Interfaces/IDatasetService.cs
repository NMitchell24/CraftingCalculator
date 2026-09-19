using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// The datasets themselves - the named containers a user switches between - as opposed to
/// <see cref="IRecordService"/>, which is the records inside the selected one.
/// </summary>
public interface IDatasetService
{
    /// <summary>Every dataset, ordered alphabetically by name.</summary>
    Task<List<DatasetModel>> GetAllAsync();

    /// <summary>
    /// Resolves the dataset to open with and publishes it to <see cref="ISelectedDatasetState"/>: the
    /// stored selection when that dataset still exists, otherwise the first dataset by name. Must run
    /// at startup, after the database has migrated and before anything reads a record.
    /// </summary>
    Task InitializeAsync();

    /// <summary>True when a dataset other than <paramref name="exceptId"/> already uses this name,
    /// compared ignoring case.</summary>
    Task<bool> NameExistsAsync(string name, int exceptId = 0);

    /// <summary>Adds an empty dataset and returns it with its assigned id.</summary>
    Task<DatasetModel> CreateAsync(string name);

    /// <summary>
    /// Adds a dataset holding its own copy of everything in <paramref name="sourceId"/> - every category,
    /// component, blueprint and favorite, and its settings - and returns it with its assigned id. The copies are independent
    /// records, so editing either dataset afterwards leaves the other alone.
    /// </summary>
    Task<DatasetModel> CopyAsync(int sourceId, string name);

    Task RenameAsync(int id, string name);

    /// <summary>
    /// Deletes the dataset and every record in it, selecting another dataset first when the deleted one
    /// was selected. Does nothing when it is the only dataset left, since one is always selected.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>Selects <paramref name="id"/> and persists the choice across launches.</summary>
    Task SwitchToAsync(int id);

    /// <summary>
    /// Applies <paramref name="change"/> to the selected dataset's saved settings, saves the result as its own and
    /// publishes it to <see cref="ISelectedDatasetState.Settings"/>. Updates are applied one at a time, in the order
    /// they were made, each to the result of the one before, so no update undoes another.
    /// </summary>
    /// <param name="change">Returns the settings with the one change made, e.g. <c>s =&gt; s with { UseYield = false }</c>.</param>
    Task UpdateSettingsAsync(Func<Datasettings, Datasettings> change);
}
