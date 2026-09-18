using CraftingCalculator.Application.BusinessLogic.Transfer.Format;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>The folder on the device that export files are saved to.</summary>
public interface IExportFileStore
{
    /// <summary>
    /// Saves <paramref name="document"/> as a new export file named after its dataset and export time, then
    /// deletes the oldest export files beyond the newest <paramref name="keep"/>. An interrupted save never
    /// leaves a partial file among the exports.
    /// </summary>
    /// <param name="document">The records to write.</param>
    /// <param name="keep">
    /// How many exports to leave in the folder, counting the one being saved. At least one.
    /// </param>
    Task<ExportFileInfo> SaveAsync(TransferDocument document, int keep);

    /// <summary>
    /// Every export file still in the folder, newest first. Reads the folder on every call, so a file deleted
    /// outside the app is never returned.
    /// </summary>
    IReadOnlyList<ExportFileInfo> List();
}
