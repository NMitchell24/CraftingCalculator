using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.Platform;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The diagnostic log as it stands on the device: where the file is, and every retained entry, oldest first.
/// On a platform with a downloads folder it can also hand the text over as a text file.
/// </summary>
public partial class LogViewerDialog : ComponentBase
{
    // A .log file has no registered media type and opens in nothing; the log is plain text, so it is saved as
    // text and every platform already knows what to do with it.
    private const string LogMimeType = "text/plain";

    private const string DownloadLabel = "Download the log";

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private IDiagnosticLog Log { get; set; } = null!;
    [Inject] private IFileDownloader Downloader { get; set; } = null!;
    [Inject] private IDialogService Dialogs { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private ILogger<LogViewerDialog> Logger { get; set; } = null!;

    // Null while the file is still being read, which is what the spinner renders on; empty is a log with
    // nothing in it yet.
    private string? _text;

    private bool _downloading;

    private string Location => DiagnosticLogLocation.Describe(Log);

    /// <summary>Opens the log viewer, full screen on a phone and a large card anywhere else.</summary>
    public static Task<IDialogReference> ShowAsync(IDialogService dialogs, Breakpoint breakpoint)
    {
        DialogOptions options = new()
        {
            FullScreen = breakpoint == Breakpoint.Xs,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        return dialogs.ShowAsync<LogViewerDialog>("Logs", options);
    }

    protected override async Task OnInitializedAsync() =>
        // The log is read with synchronous file I/O, same as SQLite, so reading it on the UI thread would hold
        // the renderer and the spinner above would never paint.
        _text = await Task.Run(Log.ReadAll);

    private async Task DownloadAsync()
    {
        // One save at a time. The cache copy is named to the second, so two overlapping runs share a path that
        // each one truncates with FileMode.Create and then deletes, and the second tap would only ever produce
        // a duplicate of the first copy anyway. The Disabled binding alone would not do it: Blazor dispatches
        // handlers serially but a second tap can arrive before the disabled state reaches the DOM, which is
        // also why this check-and-set cannot interleave.
        if (_downloading)
        {
            return;
        }

        _downloading = true;

        // What the user is looking at, not a re-read: the file may have grown since, and the two disagreeing
        // would be worse than a copy that is a few entries behind.
        string path = Path.Combine(
            FileSystem.CacheDirectory, $"crafting-calculator-log-{DateTime.Now:yyyyMMdd-HHmmss}.txt");

        try
        {
            await File.WriteAllTextAsync(path, _text);
            await Downloader.SaveToDownloadsAsync(path, LogMimeType);
            Snackbar.Add("Saved to your Downloads folder.", Severity.Success);
        }
        catch (Exception exception)
        {
            LogDownloadFailed(Logger, exception);

            await ConfirmDialog.AlertAsync(
                Dialogs,
                "The log wasn't saved",
                "The log couldn't be saved to your Downloads folder. Try again, and if it keeps failing "
                + "you can still read it here.");
        }
        finally
        {
            DeleteQuietly(path);
            _downloading = false;
        }
    }

    // The cache is the device's to clear, but a copy of the log left in it is a second place the same text
    // lives, so it goes as soon as the downloads folder has it.
    private void DeleteQuietly(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception exception)
        {
            LogTempDeleteFailed(Logger, exception);
        }
    }

    private void Close() => MudDialog.Close();

    [LoggerMessage(Level = LogLevel.Error, Message = "The log could not be saved to the Downloads folder")]
    private static partial void LogDownloadFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The log copy in the cache directory could not be deleted")]
    private static partial void LogTempDeleteFailed(ILogger logger, Exception exception);
}
