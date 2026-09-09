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

    private CraftView _view = CraftView.Materials;
    private List<BlueprintFavorite> _favorites = [];

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
        State.Changed += StateHasChanged;
        PageShellState.Configure(this, new PageShellConfig("Craft")
        {
            Actions =
            [
                new PageAction("Add blueprints", Icons.Material.Filled.Add, OpenPickerAsync),
                new PageAction("Clear selection", Icons.Material.Filled.Clear, ClearBatchAsync),
                new PageAction("Save as favorite", Icons.Material.Filled.Save, SaveAsFavoriteAsync)
            ]
        });

        await ReloadFavoritesAsync();
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
        State.Changed -= StateHasChanged;
        PageShellState.Reset(this);
    }
}
