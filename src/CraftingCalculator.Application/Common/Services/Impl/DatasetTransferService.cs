using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Services.Impl;

public class DatasetTransferService(
    IDatasetDAO datasetDAO,
    ISelectedDatasetState selectedDataset,
    IExportFileStore exportFileStore,
    TimeProvider timeProvider) : IDatasetTransferService
{
    public Task<DatasetSnapshot> LoadCurrentSnapshotAsync() => datasetDAO.GetSnapshotAsync(selectedDataset.Id);

    public Task<ExportFileInfo> ExportAsync(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, string appVersion) =>
        exportFileStore.SaveAsync(
            TransferDocumentProcessor.ToDocument(snapshot, selected, timeProvider.GetUtcNow(), appVersion));

    public ExportFileInfo? GetLatestExport() => exportFileStore.GetLatest();
}
