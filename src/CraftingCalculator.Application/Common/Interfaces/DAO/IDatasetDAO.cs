using CraftingCalculator.Domain.Models;

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
    /// blueprint and favorite in <paramref name="sourceId"/>, and returns it with its assigned
    /// <see cref="DatasetModel.Id"/>. The copies are independent records: editing one dataset's afterwards
    /// leaves the other's alone. The source dataset is not modified.
    /// </summary>
    Task<DatasetModel> CopyAsync(int sourceId, string name);

    Task RenameAsync(int id, string name);

    /// <summary>
    /// Deletes the dataset. Its categories, components, blueprints and favorites are removed by the
    /// database's cascade delete rather than by the caller.
    /// </summary>
    Task DeleteAsync(int id);

    Task<int> CountAsync();
}
