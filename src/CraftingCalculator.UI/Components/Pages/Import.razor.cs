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
        await Js.InvokeVoidAsync("scrollTo", 0, 0);
    }

    private void ConfigureShell() =>
        PageShellState.Configure(this, new PageShellConfig("Import Data")
        {
            ShowBack = true,
            Actions = ImportState.Step switch
            {
                ImportStep.Review =>
                [
                    new PageAction("Import Data", Icons.Material.Filled.Input, ImportDataAsync, Disabled: !ImportState.CanImport),
                    new PageAction("Choose Another File", Icons.Material.Filled.FileOpen, ImportState.PickFileAsync)
                ],
                ImportStep.ResolveConflicts => [new PageAction("Import Selected", Icons.Material.Filled.Input, ImportSelectedAsync)],
                _ => []
            }
        });

    // Raised by the wizard's background steps as well as by this page.
    private void OnImportChanged() => _ = InvokeAsync(() =>
    {
        // Can be queued before Dispose unsubscribes, and configuring then would take the shell from the next page.
        if (_disposed)
        {
            return;
        }

        ConfigureShell();
        StateHasChanged();
    });

    private async Task ImportDataAsync()
    {
        if (!ImportState.CanImport || ImportState.ImportFile is not { Snapshot: var snapshot })
        {
            return;
        }

        int datasetId = SelectedDataset.Id;
        string datasetName = (await Task.Run(DatasetService.GetAllAsync))
            .FirstOrDefault(dataset => dataset.Id == datasetId)?.Name ?? "";

        bool? choice = await DialogService.ShowMessageBoxAsync(
            "Import Data", $"Add it to '{datasetName}', or make a new dataset?",
            yesText: "As New Dataset", noText: "Into Current", cancelText: "Cancel");

        if (choice == true)
        {
            string? name = await DatasetPrompts.PromptForDatasetNameAsync(
                DialogService, DatasetService, "Import as New Dataset", snapshot.DatasetName.Trim());

            if (name is not null)
            {
                await ImportState.ImportAsNewAsync(name);
            }
        }
        else if (choice == false)
        {
            await ImportState.CheckConflictsAsync(datasetId, datasetName);
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
