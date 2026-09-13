using CraftingCalculator.Application.BusinessLogic.Transfer.Format.V1;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>The folder on the device that export files are saved to.</summary>
public interface IExportFileStore
{
    /// <summary>
    /// Saves <paramref name="document"/> as a new export file named after its dataset and export time, then
    /// deletes the oldest export files beyond the newest five. An interrupted save never leaves a partial
    /// file among the exports.
    /// </summary>
    Task<ExportFileInfo> SaveAsync(TransferDocumentV1 document);

    /// <summary>
    /// The most recently saved export file still in the folder, or null when there is none. Reads the folder
    /// on every call, so a file deleted outside the app is never returned.
    /// </summary>
    ExportFileInfo? GetLatest();
}
