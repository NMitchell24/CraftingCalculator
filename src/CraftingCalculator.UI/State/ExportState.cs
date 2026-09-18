using System.Diagnostics;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The export running in the background and the export files on the device: what has to outlive the Export
/// page, which the user can leave while an export runs and come back to. Scoped. The page's selection is
/// deliberately not held here; every visit starts again with everything selected.
/// </summary>
/// <remarks>
/// <see cref="Changed"/> can be raised off the renderer's dispatcher, so a component subscribes with
/// <c>_ = InvokeAsync(StateHasChanged)</c> rather than calling <c>StateHasChanged</c> directly.
/// </remarks>
public sealed class ExportState(IDatasetTransferService transferService)
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

        // Copied so the page can keep changing its selection while the export reads this one.
        HashSet<RecordKey> frozen = [.. selected];
        string appVersion = AppInfo.Current.VersionString;

        try
        {
            await Task.Run(() => transferService.ExportAsync(snapshot, frozen, appVersion));
        }
        catch (Exception exception)
        {
            // Every exception, for the reason given in ReadExportsAsync.
            Debug.WriteLine(exception);
            LastError = "Your export couldn't be saved, so no file was written. Try again.";
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
            // No ErrorBoundary exists, so an exception reaching the renderer freezes the whole app. A folder
            // that cannot be read is reported as no exports rather than as a failure.
            Debug.WriteLine(exception);
            Exports = [];
        }
    }
}
