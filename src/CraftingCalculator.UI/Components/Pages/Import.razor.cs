using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// Pick a file, choose which of its records to import, then import them as a new dataset or into the current one.
/// The wizard itself lives in <see cref="ImportState"/>, so this page shows the control for the step it has reached,
/// and the user can leave at any step and come back to it.
/// </summary>
public partial class Import : ComponentBase, IDisposable
{
    [Inject] private ImportState ImportState { get; set; } = null!;
    [Inject] private IDatasetService DatasetService { get; set; } = null!;
    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;

    private ImportStep _renderedStep;
    private bool _disposed;

    protected override void OnInitialized()
    {
        _renderedStep = ImportState.Step;
        ImportState.Changed += OnImportChanged;
        ConfigureShell();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // A new step replaces the page's content the way a navigation would, but keeps the document's scroll offset:
        // back on Select File after a long conflict list, the step card was left scrolled up under the app bar.
        if (_disposed || ImportState.Step == _renderedStep)
        {
            return;
        }

        _renderedStep = ImportState.Step;
        await Js.InvokeVoidAsync("appScroll.toTop");
    }

    private void ConfigureShell() =>
        PageShellState.Configure(this, new PageShellConfig("Import Data")
        {
            ShowBack = true,
            Actions = ImportState.Step switch
            {
                ImportStep.Review =>
                [
                    new PageAction("Import data", Icons.Material.Filled.Input, ImportDataAsync, Disabled: !ImportState.CanImport),
                    new PageAction("Choose another file", Icons.Material.Filled.FileOpen, ImportState.PickFileAsync)
                ],
                ImportStep.ResolveConflicts => [new PageAction("Import selected", Icons.Material.Filled.Input, ImportSelectedAsync)],
                _ => []
            }
        });

    // Raised by the wizard's background steps as well as by this page.
    private void OnImportChanged() => _ = FireAndForget.RunAsync(
        () => InvokeAsync(() =>
        {
            // Can be queued before Dispose unsubscribes, and configuring then would take the shell from the
            // next page.
            if (_disposed)
            {
                return;
            }

            ConfigureShell();
            StateHasChanged();
        }),
        DispatchExceptionAsync);

    // ImportState catches everything its own steps throw and reports it inline on the step card, so what
    // this guard is left with is the choices made on the way in - reading the dataset names, and the two
    // prompts - none of which has written anything yet.
    private Task ImportDataAsync() => Guard.RunAsync(
        "Import.ImportData",
        "I couldn't start the import.",
        ChooseImportTargetAsync);

    private async Task ChooseImportTargetAsync()
    {
        if (!ImportState.CanImport || ImportState.ImportFile is not { Snapshot: var snapshot })
        {
            return;
        }

        int datasetId = SelectedDataset.Id;
        string datasetName = (await Task.Run(DatasetService.GetAllAsync))
            .FirstOrDefault(dataset => dataset.Id == datasetId)?.Name ?? "";

        // A new dataset is the emphasized choice because it cannot change anything the user already has,
        // which is what import-export.md recommends; merging into the open dataset is the alternative.
        // Neither label names the dataset; the message above them does.
        bool? choice = await ConfirmDialog.ChooseAsync(
            DialogService,
            "Import data",
            $"Add it to '{datasetName}', or make a new dataset?",
            confirmText: "As new dataset", alternativeText: "Into this dataset");

        switch (choice)
        {
            case true:
            {
                string? name = await DatasetPrompts.PromptForDatasetNameAsync(
                    DialogService, DatasetService, "Import as new dataset", "Create", snapshot.DatasetName.Trim());

                if (name is not null)
                {
                    await ImportState.ImportAsNewAsync(name);
                }

                break;
            }
            case false:
                await ImportState.CheckConflictsAsync(datasetId, datasetName);
                break;
        }
    }

    private Task ImportSelectedAsync() => ImportState.MergeAsync(ImportState.Replace);

    public void Dispose()
    {
        _disposed = true;
        ImportState.Changed -= OnImportChanged;
        PageShellState.Reset(this);
    }
}
