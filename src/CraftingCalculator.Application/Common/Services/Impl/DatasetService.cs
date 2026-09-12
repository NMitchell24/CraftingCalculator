using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class DatasetService(IDatasetDAO dao, ISelectedDatasetState selectedDataset) : IDatasetService
{
    public Task<List<DatasetModel>> GetAllAsync() => dao.GetAllAsync();

    public async Task InitializeAsync()
    {
        if (selectedDataset.Id != 0 && await dao.GetByIdAsync(selectedDataset.Id) != null)
        {
            return;
        }

        // Reached on a first launch, and again whenever the stored dataset has been deleted - by this
        // app on another device restoring a backup, or by a hand-edited database. The first dataset by
        // name is the arbitrary-but-stable choice; the migration guarantees at least one exists.
        List<DatasetModel> datasets = await dao.GetAllAsync();

        if (datasets.Count == 0)
        {
            DatasetModel created = await dao.AddAsync(DatasetConstants.DefaultName);
            selectedDataset.Set(created.Id);
            return;
        }

        selectedDataset.Set(datasets[0].Id);
    }

    public async Task<bool> NameExistsAsync(string name, int exceptId = 0)
        => await dao.GetByNameAsync(name.Trim(), exceptId) != null;

    public Task<DatasetModel> CreateAsync(string name) => dao.AddAsync(name.Trim());

    public Task RenameAsync(int id, string name) => dao.RenameAsync(id, name.Trim());

    public async Task DeleteAsync(int id)
    {
        // One dataset is always selected, so the last one cannot go. The UI disables the action at the
        // same threshold; this is the invariant behind that, not a duplicate of it.
        if (await dao.CountAsync() <= 1)
        {
            return;
        }

        // Moved before the delete, not after: the cascade runs while this dataset is still selected, and
        // a context created in between would be scoped to a dataset that no longer exists.
        if (selectedDataset.Id == id)
        {
            DatasetModel replacement = (await dao.GetAllAsync()).First(dataset => dataset.Id != id);
            selectedDataset.Set(replacement.Id);
        }

        await dao.DeleteAsync(id);
    }

    public async Task SwitchToAsync(int id)
    {
        // Guards against a stale dropdown selecting a dataset another path has already deleted, which
        // would otherwise leave every screen scoped to a missing dataset and silently empty.
        if (await dao.GetByIdAsync(id) == null)
        {
            return;
        }

        selectedDataset.Set(id);
    }
}
