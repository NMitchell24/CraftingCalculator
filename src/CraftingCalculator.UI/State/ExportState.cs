using System.Diagnostics;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.UI.State;

/// <summary>
/// The export running in the background and the latest export file on the device: what has to outlive the
/// Export page, which the user can leave while an export runs and come back to. Scoped. The page's selection
/// is deliberately not held here; every visit starts again with everything selected.
/// </summary>
/// <remarks>
/// <see cref="Changed"/> can be raised off the renderer's dispatcher, so a component subscribes with
/// <c>_ = InvokeAsync(StateHasChanged)</c> rather than calling <c>StateHasChanged</c> directly.
/// </remarks>
public sealed class ExportState(IDatasetTransferService transferService)
{
    public bool IsRunning { get; private set; }

    /// <summary>The newest export file as of the last export or <see cref="RefreshLatestAsync"/>, or null.</summary>
    public ExportFileInfo? Latest { get; private set; }

    /// <summary>What went wrong with the last export, in words for the user, or null if it succeeded.</summary>
    public string? LastError { get; private set; }

    public event Action? Changed;

    /// <summary>Re-reads the newest export file from the device, so one deleted outside the app is dropped.</summary>
    public async Task RefreshLatestAsync()
    {
        try
        {
            Latest = await Task.Run(transferService.GetLatestExport);
        }
        catch (Exception exception)
        {
            // No ErrorBoundary exists, so an exception reaching the renderer freezes the whole app. A folder
            // that cannot be read is reported as no export rather than as a failure.
            Debug.WriteLine(exception);
            Latest = null;
        }

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
            Latest = await Task.Run(() => transferService.ExportAsync(snapshot, frozen, appVersion));
        }
        catch (Exception exception)
        {
            // Every exception, for the reason given in RefreshLatestAsync.
            Debug.WriteLine(exception);
            LastError = "Your export couldn't be saved, so no file was written. Try again.";
        }
        finally
        {
            IsRunning = false;
            Changed?.Invoke();
        }
    }
}
