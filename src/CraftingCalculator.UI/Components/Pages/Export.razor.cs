using System.Diagnostics;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// Choose records from the current dataset and export them to a file, then share the newest export. The
/// export itself runs in <see cref="ExportState"/>, so it carries on if the user leaves this page.
/// </summary>
public partial class Export : ComponentBase, IDisposable
{
    [Inject] private IDatasetTransferService TransferService { get; set; } = null!;
    [Inject] private IShareService ShareService { get; set; } = null!;
    [Inject] private ExportState ExportState { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

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
            // No ErrorBoundary exists, so a throw out of a load path freezes the whole app.
            Debug.WriteLine(exception);
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
            await ExportState.RefreshLatestAsync();
        }
    }

    private void ConfigureShell() =>
        PageShellState.Configure(this, new PageShellConfig("Export Data")
        {
            ShowBack = true,
            Actions =
            [
                new PageAction("Export Data", Icons.Material.Filled.Output, ExportAsync, Disabled: !CanExport),
                new PageAction("Share", Icons.Material.Filled.Share, ShareAsync,
                    Disabled: ExportState.Latest is null || ExportState.IsRunning)
            ]
        });

    // Raised by the export's background task as well as by this page.
    private void OnExportChanged() => _ = InvokeAsync(() =>
    {
        ConfigureShell();
        StateHasChanged();
    });

    private Task ExportAsync() =>
        CanExport ? ExportState.StartAsync(_snapshot!, _selected) : Task.CompletedTask;

    private async Task ShareAsync()
    {
        // Re-read first: the file may have been deleted since the page last looked.
        await ExportState.RefreshLatestAsync();

        if (ExportState.Latest is not { } latest)
        {
            Snackbar.Add("That export file is gone. Export again to make a new one.", Severity.Warning);
            return;
        }

        try
        {
            await ShareService.ShareFileAsync(latest.FullPath, "Share your export");
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            Snackbar.Add("The share sheet couldn't be opened.", Severity.Error);
        }
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
}
