using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// Choose records from the current dataset and export them to a file, and download or share any export still
/// on the device. The export itself runs in <see cref="ExportState"/>, so it carries on if the user leaves this page.
/// </summary>
public partial class Export : ComponentBase, IDisposable
{
    [Inject] private IDatasetTransferService TransferService { get; set; } = null!;
    [Inject] private IFileDownloader Downloader { get; set; } = null!;
    [Inject] private IShareService ShareService { get; set; } = null!;
    [Inject] private ExportState ExportState { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;
    [Inject] private ILogger<Export> Logger { get; set; } = null!;

    // .ccdata is the app's own extension, so no registered media type describes it. The downloads folder
    // requires one, and this is what Android itself falls back to for a file it cannot identify.
    private const string ExportMimeType = "application/octet-stream";

    private DatasetSnapshot? _snapshot;
    private DependencyGraph? _graph;
    private IReadOnlySet<RecordKey> _cyclic = new HashSet<RecordKey>();
    private string? _loadError;
    private readonly HashSet<RecordKey> _selected = [];
    private bool _disposed;

    /// <summary>The selected blueprints that end up nested inside themselves, which no export may contain.</summary>
    private List<string> SelectedCycleNames =>
        _cyclic.Count == 0 || _snapshot is null
            ? []
            :
            [
                .. _snapshot.Blueprints
                    .Select(blueprint => (Key: new RecordKey(RecordKind.Blueprint, blueprint.Id), blueprint.Name))
                    .Where(blueprint => _cyclic.Contains(blueprint.Key) && _selected.Contains(blueprint.Key))
                    .Select(blueprint => $"'{blueprint.Name}'")
            ];

    private bool CanExport => _snapshot is not null && _selected.Count > 0 && !ExportState.IsRunning && SelectedCycleNames.Count == 0;

    // An export with a loop in it could never be imported, so it is refused rather than written. Deselecting
    // the blueprints on the loop is enough to export the rest.
    private string? CycleMessage
    {
        get
        {
            List<string> names = SelectedCycleNames;

            return names.Count switch
            {
                0 => null,
                1 => $"{names[0]} is nested inside itself, so it can't be exported. Fix it in the blueprint editor "
                     + "or deselect it, then export again.",
                _ => $"{string.Join(", ", names[..^1])} and {names[^1]} are nested inside each other, so they can't "
                     + "be exported. Fix one of them in the blueprint editor or deselect them, then export again."
            };
        }
    }

    protected override async Task OnInitializedAsync()
    {
        ExportState.Changed += OnExportChanged;
        ConfigureShell();

        try
        {
            // One background hop for the whole load: SQLite blocks the thread it runs on, and building the graph
            // of a large dataset is not free either.
            (_snapshot, _graph, _cyclic) = await Task.Run(async () =>
            {
                DatasetSnapshot snapshot = await TransferService.LoadCurrentSnapshotAsync();
                DependencyGraph graph = DependencyGraphProcessor.Build(snapshot);

                return (snapshot, graph, DependencyGraphProcessor.FindCycles(graph));
            });

            _selected.UnionWith(_graph.All);
        }
        catch (Exception exception)
        {
            // Caught rather than left to the page boundary: the screen states the failure in its own words,
            // and stays up with its back arrow rather than becoming a generic error card.
            LogSnapshotLoadFailed(Logger, exception);
            _loadError = "Your dataset couldn't be loaded for export. Go back and try again.";
        }

        // The user can leave while the load runs. Blazor configures the incoming page before disposing this
        // one, so configuring now would take the shell back from that page with nothing left to reset it.
        if (_disposed)
        {
            return;
        }

        ConfigureShell();

        if (!ExportState.IsRunning)
        {
            await ExportState.RefreshExportsAsync();
        }
    }

    private void ConfigureShell() =>
        PageShellState.Configure(this, new PageShellConfig("Export Data")
        {
            ShowBack = true,
            Actions = [new PageAction("Export data", Icons.Material.Filled.Output, ExportAsync, Disabled: !CanExport)]
        });

    // Raised by the export's background task as well as by this page.
    private void OnExportChanged() => _ = FireAndForget.RunAsync(
        () => InvokeAsync(() =>
        {
            // Same guard as Import.OnImportChanged, and for the same reason: ExportState outlives this page,
            // so a callback queued before Dispose unsubscribes can land after the incoming page has already
            // configured the shared shell, and ConfigureShell would take the title and actions off it.
            if (_disposed)
            {
                return;
            }

            ConfigureShell();
            StateHasChanged();
        }),
        DispatchExceptionAsync);

    // The actions bar runs its page actions through a guard of its own, so it takes this unguarded: a second guard
    // inside would log every export twice.
    private Task ExportAsync() =>
        CanExport ? ExportState.StartAsync(_snapshot!, _selected) : Task.CompletedTask;

    // The inline button is page markup, not a bar action, so nothing guards it but this. A failed write is
    // ExportState's to report inline; this catches a throw before the export gets that far.
    private Task GuardedExportAsync() =>
        Guard.RunAsync("Export.Start", "I couldn't start your export.", ExportAsync);

    private static string DownloadLabel(ExportFileInfo export) => $"Download {export.FileName}";

    private static string ShareLabel(ExportFileInfo export) => $"Share {export.FileName}";

    private async Task DownloadAsync(ExportFileInfo export)
    {
        if (!await StillOnTheDeviceAsync(export))
        {
            return;
        }

        bool saved = await Guard.RunAsync(
            "Export.Download",
            "I couldn't save that export to your Downloads folder.",
            () => Downloader.SaveToDownloadsAsync(export.FullPath, ExportMimeType));

        if (saved)
        {
            Snackbar.Add("Saved to your Downloads folder.", Severity.Success);
        }
    }

    private async Task ShareAsync(ExportFileInfo export)
    {
        if (!await StillOnTheDeviceAsync(export))
        {
            return;
        }

        await Guard.RunAsync(
            "Export.Share",
            "I couldn't open the share sheet.",
            () => ShareService.ShareFileAsync(export.FullPath, "Share your export"));
    }

    /// <summary>
    /// Whether <paramref name="export"/> is still on the device, telling the user when it is not. The export
    /// history is re-read first, so a file deleted since the page last looked is caught here rather than by
    /// the action failing.
    /// </summary>
    private async Task<bool> StillOnTheDeviceAsync(ExportFileInfo export)
    {
        await ExportState.RefreshExportsAsync();

        if (ExportState.Exports.Any(file => file.FullPath == export.FullPath))
        {
            return true;
        }

        // A dialog rather than a toast: the button the user tapped did not happen, and the way forward is
        // to export again.
        await ConfirmDialog.AlertAsync(
            DialogService,
            "That export is gone",
            "That export file isn't on the device any more. Export again to make a new one.");

        return false;
    }

    // Where the user can find the file, which only a PC can show as a path: the Android folder is invisible to
    // every other app, and the iOS sandbox path is not one the Files app displays.
#if WINDOWS
    private static string LocationOf(ExportFileInfo file) => Path.GetDirectoryName(file.FullPath) ?? file.FullPath;
#elif IOS
    private static string LocationOf(ExportFileInfo _) =>
        $"Files → On My {(DeviceInfo.Idiom == DeviceIdiom.Tablet ? "iPad" : "iPhone")} → Crafting Calculator → Exports";
#else
    private static string LocationOf(ExportFileInfo _) => "Saved in app storage";
#endif

    public void Dispose()
    {
        _disposed = true;
        ExportState.Changed -= OnExportChanged;
        PageShellState.Reset(this);
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "The dataset snapshot could not be loaded for export")]
    private static partial void LogSnapshotLoadFailed(ILogger logger, Exception exception);

}
