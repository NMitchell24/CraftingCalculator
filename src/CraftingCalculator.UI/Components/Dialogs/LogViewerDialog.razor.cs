using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.Logging;
using CraftingCalculator.UI.Platform;
using CraftingCalculator.UI.State;
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
    private const string DownloadLabel = "Download the log";

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private IDiagnosticLog Log { get; set; } = null!;
    [Inject] private IFileDownloader Downloader { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;
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
        // One save at a time, of text there is. The Disabled binding alone would not do it: Blazor dispatches
        // handlers serially but a second tap can arrive before the disabled state reaches the DOM, which is
        // also why this check-and-set cannot interleave.
        if (_downloading || _text is not { } text)
        {
            return;
        }

        _downloading = true;

        try
        {
            // What the user is looking at, not a re-read: the file may have grown since, and the two
            // disagreeing would be worse than a copy that is a few entries behind.
            // Its own closing line: the guard's default sends the user to the log in Settings, which is the
            // screen they are standing on. Holding the text opens the WebView's own selection menu, which
            // offers Select all and Copy - checked on the emulator, since the advice is only worth giving if
            // the gesture is really there.
            bool saved = await Guard.RunAsync(
                "LogViewer.Download",
                "I couldn't save the log to your Downloads folder.",
                () => DiagnosticLogDownload.SaveAsync(Downloader, Logger, text),
                help: "You can still get it out by hand: press and hold the log, pick Select all, then Copy.");

            if (saved)
            {
                Snackbar.Add("Saved to your Downloads folder.", Severity.Success);
            }
        }
        finally
        {
            _downloading = false;
        }
    }

    private void Close() => MudDialog.Close();
}
