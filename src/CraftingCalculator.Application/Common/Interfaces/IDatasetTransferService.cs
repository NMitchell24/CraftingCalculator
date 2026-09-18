using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>Exports records from the selected dataset to a file, and imports records read from one.</summary>
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

    /// <summary>Every export file on the device, newest first.</summary>
    IReadOnlyList<ExportFileInfo> ListExports();

    /// <summary>
    /// Adds a dataset named <paramref name="name"/>, trimmed, holding every record of <paramref name="incoming"/>,
    /// and returns it. The selected dataset stays selected.
    /// </summary>
    Task<DatasetModel> ImportAsNewAsync(DatasetSnapshot incoming, string name);

    /// <summary>
    /// The records of <paramref name="incoming"/> that share a name with a record of the same kind in
    /// <paramref name="datasetId"/>.
    /// </summary>
    Task<IReadOnlyList<ImportConflict>> FindConflictsAsync(int datasetId, DatasetSnapshot incoming);

    /// <summary>
    /// Merges <paramref name="incoming"/> into <paramref name="datasetId"/>. A record that shares a name with one
    /// already there lands on it, replacing it when the record's key is in <paramref name="replace"/> and leaving it
    /// as it is otherwise; every other record is added. Returns an empty list once the merge is written. When the
    /// merge would leave blueprints nested inside themselves, nothing is written and their names are returned.
    /// </summary>
    Task<IReadOnlyList<string>> MergeAsync(int datasetId, DatasetSnapshot incoming, IReadOnlySet<RecordKey> replace);
}
