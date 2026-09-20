using CraftingCalculator.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.UI.Logging;

/// <summary>
/// Hands diagnostic log text to the platform's downloads folder, on the platforms that have one.
/// </summary>
internal static partial class DiagnosticLogDownload
{
    // A .log file has no registered media type and opens in nothing; the log is plain text, so it is saved as
    // text and every platform already knows what to do with it.
    private const string LogMimeType = "text/plain";

    /// <summary>
    /// Saves <paramref name="text" /> to the downloads folder as a timestamped text file. Throws whatever the
    /// write or the platform's downloader throws, leaving the caller to decide how that is shown.
    /// </summary>
    internal static async Task SaveAsync(IFileDownloader downloader, ILogger logger, string text)
    {
        // The copy is named to the second, so two overlapping saves would share a path that each truncates
        // with FileMode.Create and then deletes. Every caller keeps one save in flight at a time.
        string path = Path.Combine(
            FileSystem.CacheDirectory, $"crafting-calculator-log-{DateTime.Now:yyyyMMdd-HHmmss}.txt");

        try
        {
            await File.WriteAllTextAsync(path, text);
            await downloader.SaveToDownloadsAsync(path, LogMimeType);
        }
        finally
        {
            DeleteQuietly(path, logger);
        }
    }

    // The cache is the device's to clear, but a copy of the log left in it is a second place the same text
    // lives, so it goes as soon as the downloads folder has it.
    private static void DeleteQuietly(string path, ILogger logger)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception exception)
        {
            LogTempDeleteFailed(logger, exception);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "The log copy in the cache directory could not be deleted")]
    private static partial void LogTempDeleteFailed(ILogger logger, Exception exception);
}
