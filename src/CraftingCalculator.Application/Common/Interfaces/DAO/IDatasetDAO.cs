using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Interfaces.DAO;

public interface IDatasetDAO
{
    /// <summary>Returns every dataset, ordered alphabetically by name.</summary>
    Task<List<DatasetModel>> GetAllAsync();

    Task<DatasetModel?> GetByIdAsync(int id);

    /// <summary>
    /// Returns the dataset whose name matches <paramref name="name"/> ignoring case and ignoring
    /// <paramref name="exceptId"/>, or null when no other dataset uses that name.
    /// </summary>
    Task<DatasetModel?> GetByNameAsync(string name, int exceptId);

    /// <summary>Adds a dataset and returns it with its assigned <see cref="DatasetModel.Id"/>.</summary>
    Task<DatasetModel> AddAsync(string name);

    /// <summary>
    /// Adds a dataset named <paramref name="name"/> holding its own copy of every category, component,
    /// blueprint and favorite in <paramref name="sourceId"/>, and its settings, and returns it with its assigned
    /// <see cref="DatasetModel.Id"/>. The copies are independent records: editing one dataset's afterwards
    /// leaves the other's alone. The source dataset is not modified.
    /// </summary>
    Task<DatasetModel> CopyAsync(int sourceId, string name);

    /// <summary>
    /// Every category, component, blueprint and favorite in <paramref name="datasetId"/>, with the links
    /// between them, each list in id order, and the dataset's settings. The dataset does not have to be the
    /// selected one.
    /// </summary>
    Task<DatasetSnapshot> GetSnapshotAsync(int datasetId);

    /// <summary>
    /// Adds a dataset named <paramref name="name"/> holding every record of <paramref name="snapshot"/>, linked
    /// the way the snapshot links them and with the snapshot's settings, and returns it with its assigned <see cref="DatasetModel.Id"/>. Nothing is
    /// added when any part of it fails.
    /// </summary>
    Task<DatasetModel> ImportAsNewAsync(string name, DatasetSnapshot snapshot);

    /// <summary>
    /// Writes <paramref name="plan"/> into <paramref name="datasetId"/> the way <see cref="MergePlan"/> describes.
    /// The dataset keeps its own settings. Nothing is written when any part of it fails. The dataset does not have to be the selected one.
    /// </summary>
    Task MergeAsync(int datasetId, MergePlan plan);

    Task RenameAsync(int id, string name);

    /// <summary>Replaces the settings of dataset <paramref name="id"/>.</summary>
    Task SetSettingsAsync(int id, Datasettings settings);

    /// <summary>
    /// Deletes the dataset. Its categories, components, blueprints and favorites are removed by the
    /// database's cascade delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);

    Task<int> CountAsync();
}
