namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>
/// Copies a file out of the app's own storage into the device's downloads folder. Implemented in the UI
/// project against the platform's storage API so Application has no MAUI dependency.
/// </summary>
public interface IFileDownloader
{
    /// <summary>Whether this platform has a downloads folder to copy a file into.</summary>
    bool IsSupported { get; }

    /// <summary>
    /// Copies the file at <paramref name="path"/> into the downloads folder under the same file name, or a
    /// numbered variant of it where the folder already holds a file of that name.
    /// </summary>
    /// <param name="path">The file to copy. Left where it is.</param>
    /// <param name="mimeType">The media type the downloads folder records the copy under.</param>
    /// <exception cref="NotSupportedException">
    /// This platform has no downloads folder, which <see cref="IsSupported"/> reports.
    /// </exception>
    Task SaveToDownloadsAsync(string path, string mimeType);
}
