namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Copies an export file out of the app's own storage into the device's downloads folder. Implemented in the
/// UI project against the platform's storage API so Application has no MAUI dependency.
/// </summary>
public interface IExportDownloader
{
    /// <summary>Whether this platform has a downloads folder to copy an export into.</summary>
    bool IsSupported { get; }

    /// <summary>
    /// Copies the export at <paramref name="path"/> into the downloads folder under the same file name, or a
    /// numbered variant of it where the folder already holds a file of that name.
    /// </summary>
    /// <param name="path">The export file to copy. Left where it is.</param>
    /// <exception cref="NotSupportedException">
    /// This platform has no downloads folder, which <see cref="IsSupported"/> reports.
    /// </exception>
    Task SaveToDownloadsAsync(string path);
}
