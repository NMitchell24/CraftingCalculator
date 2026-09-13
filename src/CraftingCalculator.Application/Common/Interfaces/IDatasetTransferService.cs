using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>Exports records from the selected dataset to a file.</summary>
public interface IDatasetTransferService
{
    /// <summary>Every record of the selected dataset.</summary>
    Task<DatasetSnapshot> LoadCurrentSnapshotAsync();

    /// <summary>
    /// Saves the <paramref name="selected"/> records of <paramref name="snapshot"/> to a new export file and
    /// returns it. <paramref name="appVersion"/> is recorded in the file.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="selected"/> leaves out a record that a selected record depends on.
    /// </exception>
    Task<ExportFileInfo> ExportAsync(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected, string appVersion);

    /// <summary>The most recent export file on the device, or null when there is none.</summary>
    ExportFileInfo? GetLatestExport();
}
