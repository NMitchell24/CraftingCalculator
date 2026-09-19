using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.Common.Services.Impl;

public sealed class DatasetService(IDatasetDAO dao, ISelectedDatasetState selectedDataset) : IDatasetService, IDisposable
{
    private readonly SemaphoreSlim _settingsGate = new(1, 1);

    public Task<List<DatasetModel>> GetAllAsync() => dao.GetAllAsync();

    public async Task InitializeAsync()
    {
        // Selected again even though the id is unchanged: the stored preference carries only the id, and this is
        // what loads the dataset's settings.
        if (selectedDataset.Id != 0 && await dao.GetByIdAsync(selectedDataset.Id) is { } stored)
        {
            Select(stored);
            return;
        }

        // Reached on a first launch, and again whenever the stored dataset has been deleted - by this
        // app on another device restoring a backup, or by a hand-edited database. The first dataset by
        // name is the arbitrary-but-stable choice; the migration guarantees at least one exists.
        List<DatasetModel> datasets = await dao.GetAllAsync();

        if (datasets.Count == 0)
        {
            DatasetModel created = await dao.AddAsync(DatasetConstants.DefaultName);
            Select(created);
            return;
        }

        Select(datasets[0]);
    }

    public async Task<bool> NameExistsAsync(string name, int exceptId = 0)
        => await dao.GetByNameAsync(name.Trim(), exceptId) != null;

    public Task<DatasetModel> CreateAsync(string name) => dao.AddAsync(name.Trim());

    public Task<DatasetModel> CopyAsync(int sourceId, string name) => dao.CopyAsync(sourceId, name.Trim());

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
            Select(replacement);
        }

        await dao.DeleteAsync(id);
    }

    public async Task SwitchToAsync(int id)
    {
        // Guards against a stale dropdown selecting a dataset another path has already deleted, which
        // would otherwise leave every screen scoped to a missing dataset and silently empty.
        if (await dao.GetByIdAsync(id) is not { } dataset)
        {
            return;
        }

        Select(dataset);
    }

    public async Task UpdateSettingsAsync(Func<Datasettings, Datasettings> change)
    {
        int id = selectedDataset.Id;

        // The toggles save off the UI thread, so a second tap can land while the first write is still running. One
        // update at a time, each reading the row the last one wrote: two updates that both started from the settings
        // published before either would each write back the other's setting unchanged, and the later write would undo
        // the earlier change. Read from the row rather than from selectedDataset, which a switch may already have
        // moved to another dataset.
        await _settingsGate.WaitAsync();
        try
        {
            // Null when the dataset was deleted while this update waited its turn.
            if (await dao.GetByIdAsync(id) is not { } dataset)
            {
                return;
            }

            Datasettings settings = change(dataset.Settings);
            await dao.SetSettingsAsync(id, settings);

            // A switch can land during the write. It has already published the new dataset's own settings, and these
            // belong to the dataset that was selected when the update started.
            if (selectedDataset.Id == id)
            {
                selectedDataset.Set(id, settings);
            }
        }
        finally
        {
            _settingsGate.Release();
        }
    }

    public void Dispose() => _settingsGate.Dispose();

    private void Select(DatasetModel dataset) => selectedDataset.Set(dataset.Id, dataset.Settings);
}
