using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Craft : ComponentBase, IDisposable
{
    private enum CraftView
    {
        Materials,
        Steps,
        Surplus
    }

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IClipboardService ClipboardService { get; set; } = null!;
    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;

    /// <summary>One tap of a batch row's + or - in the normal mode.</summary>
    private const long SingleStep = 1;

    /// <summary>One tap of a batch row's + or - while the x10 mode is on.</summary>
    private const long BulkStep = 10;

    // Hand-rolled: Material's numbered icons stop at LooksTwo, and _10k draws "10K". A multiplication
    // sign followed by the digits 1 and 0, drawn for MudBlazor's 24x24 icon viewBox.
    // docs/help/assets/x10.svg wraps this same markup for the help pages.
    private const string TimesTenIcon =
        "<path d=\"M0 0h24v24H0z\" fill=\"none\"/>" +
        "<path d=\"M8.5 9.21L7.8 8.5 5 11.3 2.21 8.5 1.5 9.21 4.3 12 1.5 14.8 2.21 15.5 5 12.71 7.8 15.5 8.5 14.8 5.71 12z\"/>" +
        "<path d=\"M14 6h-1.4L10 8.4v1.7l2.6-1.9V18h1.4z\"/>" +
        "<path d=\"M19 6a3.2 6 0 1 0 0 12 3.2 6 0 1 0 0-12zm0 2.4a1.6 3.6 0 1 1 0 7.2 1.6 3.6 0 1 1 0-7.2z\"/>";

    private CraftView _view = CraftView.Materials;
    private List<BlueprintFavorite> _favorites = [];
    private long _stepSize = SingleStep;

    private int _favoriteSelectNonce;

    // Only drives the Add Blueprints dialog's FullScreen vs. side-drawer choice now - the pane layout
    // itself (single active pane vs. all three side by side) is a pure CSS media query (app.css),
    // since it needs both width and height to tell a landscape phone apart from a real tablet.
    private bool IsXs => Breakpoint == Breakpoint.Xs;

    // Reflects CraftState rather than holding a selection of its own, so clearing the batch or loading a
    // favorite from anywhere else moves the select with it.
    private int? SelectedFavoriteId =>
        _favorites.FirstOrDefault(favorite => favorite.Name == State.LoadedFavoriteName)?.Id;

    private string FavoriteSelectLabel => _favorites.Count == 0 ? "No Favorites" : "Load Favorite";

    // The Load Favorite select is rebuilt rather than updated: MudSelect holds the value it set itself
    // and does not reliably take a new one back from its Value parameter, which showed up as the field
    // naming a favorite that was never loaded. The id covers a selection made elsewhere - saving a new
    // favorite, clearing the batch; the nonce covers the case the id cannot, where backing out of the
    // replace confirm leaves SelectedFavoriteId exactly where it already was.
    private (int Nonce, int? FavoriteId) FavoriteSelectKey => (_favoriteSelectNonce, SelectedFavoriteId);

    protected override async Task OnInitializedAsync()
    {
        State.Changed += OnStateChanged;
        ConfigureShell();

        await ReloadFavoritesAsync();
    }

    // Re-declared rather than configured once, because the x10 action carries both the mode it is in and
    // whether the batch has anything to step.
    private void ConfigureShell()
    {
        PageShellState.Configure(this, new PageShellConfig("Craft")
        {
            Actions =
            [
                new PageAction("Add blueprints", Icons.Material.Filled.Add, OpenPickerAsync),
                new PageAction("Clear selection", Icons.Material.Filled.Clear, ClearBatchAsync),
                new PageAction("Save as favorite", Icons.Material.Filled.Save, SaveAsFavoriteAsync),
                new PageAction("Step by 10", TimesTenIcon, ToggleStepSizeAsync,
                    Disabled: State.BlueprintQuantities.Count == 0, Active: _stepSize == BulkStep)
            ]
        });
    }

    private void OnStateChanged()
    {
        // An emptied batch drops the mode rather than leaving it armed for whatever is added next: the
        // action is disabled while there is nothing to step, so there would be no way to turn it off.
        if (State.BlueprintQuantities.Count == 0)
        {
            _stepSize = SingleStep;
        }

        ConfigureShell();
        StateHasChanged();
    }

    /// <summary>Switches the batch steppers between stepping by one and stepping by ten.</summary>
    private Task ToggleStepSizeAsync()
    {
        _stepSize = _stepSize == SingleStep ? BulkStep : SingleStep;
        ConfigureShell();

        if (_stepSize == BulkStep)
        {
            Snackbar.Add("Stepping by 10. Tap x10 again to go back to 1", Severity.Info);
        }

        // ActionsBar owns the action's click, so its EventCallback renders that component and never this
        // page - the steppers would otherwise keep the previous mode's icons.
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task ReloadFavoritesAsync()
    {
        _favorites = await FavoriteService.GetAllFavoritesAsync();

        // Rendered here rather than left to the caller: ActionsBar owns the Save action's click, so its
        // EventCallback renders that component and never this page.
        StateHasChanged();
    }

    private async Task OnFavoriteSelectedAsync(int? favoriteId)
    {
        _favoriteSelectNonce++;

        if (_favorites.FirstOrDefault(favorite => favorite.Id == favoriteId) is { } selected)
        {
            await FavoritePrompts.LoadAsync(DialogService, Snackbar, State, selected);
        }
    }

    private void OnViewChanged(CraftView view) => _view = view;

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

        if (result is { Canceled: false } && result.Data is IReadOnlyCollection<BlueprintModel> selected)
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
        string text = string.Join(Environment.NewLine, State.TotalComponents.Select(componentQuantity => componentQuantity.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied components to clipboard", Severity.Success);
    }

    private async Task CopySurplusAsync()
    {
        string text = string.Join(Environment.NewLine, State.SurplusStock.Select(blueprintQuantity => blueprintQuantity.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied surplus to clipboard", Severity.Success);
    }

    private async Task SaveAsFavoriteAsync()
    {
        if (await FavoritePrompts.SaveBatchAsync(DialogService, Snackbar, State) is not null)
        {
            await ReloadFavoritesAsync();
        }
    }

    public void Dispose()
    {
        State.Changed -= OnStateChanged;
        PageShellState.Reset(this);
    }
}
