using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models.Transfer;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The export running in the background and the export files on the device: what has to outlive the Export
/// page, which the user can leave while an export runs and come back to. Scoped. The page's selection is
/// deliberately not held here; every visit starts again with everything selected.
/// </summary>
/// <remarks>
/// <see cref="Changed"/> can be raised off the renderer's dispatcher, so a component subscribes with
/// <c>_ = FireAndForget.RunAsync(() =&gt; InvokeAsync(StateHasChanged), DispatchExceptionAsync)</c> rather
/// than calling <c>StateHasChanged</c> directly.
/// </remarks>
public sealed partial class ExportState(IDatasetTransferService transferService, ILogger<ExportState> logger)
{
    public bool IsRunning { get; private set; }

    /// <summary>
    /// The export files on the device as of the last export or <see cref="RefreshExportsAsync"/>, newest
    /// first, or empty when there are none.
    /// </summary>
    public IReadOnlyList<ExportFileInfo> Exports { get; private set; } = [];

    /// <summary>What went wrong with the last export, in words for the user, or null if it succeeded.</summary>
    public string? LastError { get; private set; }

    public event Action? Changed;

    /// <summary>Re-reads the export files from the device, so ones deleted outside the app are dropped.</summary>
    public async Task RefreshExportsAsync()
    {
        await ReadExportsAsync();
        Changed?.Invoke();
    }

    /// <summary>
    /// Saves the <paramref name="selected"/> records of <paramref name="snapshot"/> to a new export file on a
    /// background thread. Completes when the export has finished or failed, and never throws: a failure is
    /// reported through <see cref="LastError"/>.
    /// </summary>
    public async Task StartAsync(DatasetSnapshot snapshot, IReadOnlySet<RecordKey> selected)
    {
        // The page disables its export action while one runs, but a second tap can land before that render.
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;
        LastError = null;
        Changed?.Invoke();

        try
        {
            // Copied so the page can keep changing its selection while the export reads this one. Inside the try
            // with everything else after IsRunning is set: a throw that escaped here would leave it set, and the
            // page's export actions disabled until the app restarts.
            HashSet<RecordKey> frozen = [.. selected];
            LogExportStarted(logger, frozen.Count);
            string appVersion = AppInfo.Current.VersionString;

            await Task.Run(() => transferService.ExportAsync(snapshot, frozen, appVersion));
        }
        catch (Exception exception)
        {
            // Every exception, for the reason given in ReadExportsAsync.
            LogExportFailed(logger, exception);
            LastError = "I couldn't save your export.";
        }

        // Read the folder back rather than prepend the saved file: the save also deletes the oldest exports
        // beyond the number the user keeps.
        await ReadExportsAsync();

        IsRunning = false;
        Changed?.Invoke();
    }

    private async Task ReadExportsAsync()
    {
        try
        {
            Exports = await Task.Run(transferService.ListExports);
        }
        catch (Exception exception)
        {
            // Caught rather than left to the page boundary: a folder that cannot be read is reported as no
            // exports, which is still a screen the user can export from, where the boundary would replace the
            // whole page with an error card.
            LogExportsUnreadable(logger, exception);
            Exports = [];
        }
    }

    [LoggerMessage(Level = LogLevel.Trace, Message = "Export started ({Count} records)")]
    private static partial void LogExportStarted(ILogger logger, int count);

    [LoggerMessage(Level = LogLevel.Error, Message = "The export could not be written; no file was saved")]
    private static partial void LogExportFailed(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "The exports folder could not be read; reporting no exports")]
    private static partial void LogExportsUnreadable(ILogger logger, Exception exception);
}
