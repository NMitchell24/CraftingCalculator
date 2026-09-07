using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Calculate : ComponentBase, IDisposable
{
    private enum CalculateView
    {
        Materials,
        Steps
    }

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    [Inject] private CalculatorState State { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IClipboardService ClipboardService { get; set; } = null!;

    private CalculateView _view = CalculateView.Materials;

    // Only drives the Add Blueprints dialog's FullScreen vs. side-drawer choice now - the pane layout
    // itself (single active pane vs. all three side by side) is a pure CSS media query (app.css),
    // since it needs both width and height to tell a landscape phone apart from a real tablet.
    private bool IsXs => Breakpoint == Breakpoint.Xs;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
        AppBarState.Configure(this, new AppBarConfig("Craft")
        {
            PrimaryAction = new AppBarAction("Add blueprints", Icons.Material.Filled.Add, OpenPickerAsync),
            MenuItems =
            [
                new AppBarMenuItem("Clear selection", Icons.Material.Filled.ClearAll, ClearBatchAsync),
                new AppBarMenuItem("Copy components", Icons.Material.Filled.ContentCopy, CopyMaterialsAsync),
                new AppBarMenuItem("Save as favorite", Icons.Material.Filled.Star, SaveAsFavoriteAsync)
            ]
        });
    }

    private void OnViewChanged(CalculateView view) => _view = view;

    private async Task OpenPickerAsync()
    {
        DialogOptions options = new()
        {
            FullScreen = IsXs,
            MaxWidth = MaxWidth.Small,
            CloseOnEscapeKey = true
        };

        IDialogReference dialogRef = await DialogService.ShowAsync<BlueprintPickerDialog>("Select Blueprints to Craft", options);
        DialogResult? result = await dialogRef.Result;

        if (result is { Canceled: false } && result.Data is IReadOnlyCollection<Blueprint> selected)
        {
            State.AddBlueprints(selected);
        }
    }

    private Task ClearBatchAsync()
    {
        State.Clear();
        return Task.CompletedTask;
    }

    private async Task CopyMaterialsAsync()
    {
        string text = string.Join(Environment.NewLine, State.TotalComponents.Select(i => i.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied components to clipboard", Severity.Success);
    }

    private Task SaveAsFavoriteAsync() => FavoritePrompts.SaveBatchAsync(DialogService, Snackbar, State);

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
        AppBarState.Reset(this);
    }
}
