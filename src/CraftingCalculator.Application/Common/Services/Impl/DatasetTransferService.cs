using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class DatasetTransferService(
    IDatasetDAO datasetDAO,
    ISelectedDatasetState selectedDataset,
    IExportFileStore exportFileStore,
    IExportSettings exportSettings,
    TimeProvider timeProvider) : IDatasetTransferService
{
    public Task<DatasetSnapshot> LoadCurrentSnapshotAsync() => datasetDAO.GetSnapshotAsync(selectedDataset.Id);

    public Task<ExportFileInfo> ExportAsync(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, string appVersion) =>
        exportFileStore.SaveAsync(
            TransferDocumentProcessor.ToDocument(snapshot, selected, timeProvider.GetUtcNow(), appVersion),
            exportSettings.KeptExports);

    public IReadOnlyList<ExportFileInfo> ListExports() => exportFileStore.List();

    public Task<DatasetModel> ImportAsNewAsync(DatasetSnapshot incoming, string name) =>
        datasetDAO.ImportAsNewAsync(name.Trim(), incoming);

    public async Task<IReadOnlyList<ImportConflict>> FindConflictsAsync(int datasetId, DatasetSnapshot incoming) =>
        ImportConflictProcessor.Find(incoming, await datasetDAO.GetSnapshotAsync(datasetId));

    public async Task<IReadOnlyList<string>> MergeAsync(
        int datasetId, DatasetSnapshot incoming, IReadOnlySet<RecordKey> replace)
    {
        // The conflicts are found again rather than passed in: the dataset can be edited while the user is choosing,
        // and the plan has to match the rows it is about to write to.
        DatasetSnapshot current = await datasetDAO.GetSnapshotAsync(datasetId);
        MergePlan plan = new(incoming, ImportConflictProcessor.Find(incoming, current), replace);

        IReadOnlyList<string> cycles = ImportConflictProcessor.FindNewCycles(plan, current);

        if (cycles.Count == 0)
        {
            await datasetDAO.MergeAsync(datasetId, plan);
        }

        return cycles;
    }
}
