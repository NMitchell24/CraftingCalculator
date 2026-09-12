using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The saved favorites. A row opens the rename dialog, except while the list is in the Delete Mode
/// described by <see cref="ListMode"/>, where a row tap feeds that mode instead.
/// </summary>
public partial class Favorites : ComponentBase, IDisposable
{
    /// <summary>What a row tap does, driven by the Delete action on the shell.</summary>
    private enum ListMode
    {
        /// <summary>A row tap opens the rename dialog.</summary>
        Normal,

        /// <summary>Row taps toggle selection; the Delete action then deletes the selection.</summary>
        Delete
    }

    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private List<BlueprintFavorite> _favorites = [];
    private ListMode _mode = ListMode.Normal;

    // Ids rather than favorites: ReloadAsync replaces every instance, and these models have no value
    // equality, so a selection held as favorites would not survive a reload.
    private readonly HashSet<int> _selected = [];

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    // Declares no New action: a favorite is made from a batch, and the batch lives on the Craft screen,
    // which owns saving it. This screen only renames and deletes what is already saved.
    private void ConfigureShell()
    {
        bool empty = _favorites.Count == 0;

        PageShellState.Configure(this, new PageShellConfig("Favorites")
        {
            Actions =
            [
                // Wired only while the mode is on, the one state the gesture means anything in. A hold
                // outside it is inert either way - the WebView delivers no click after a long press, so
                // that tap is lost whether or not a handler is attached.
                new PageAction("Delete", Icons.Material.Filled.Delete, ToggleDeleteModeAsync,
                    Disabled: empty, Active: _mode == ListMode.Delete,
                    OnLongPress: _mode == ListMode.Delete ? ExitDeleteModeAsync : null),
                new PageAction("Delete all favorites", Icons.Material.Filled.DeleteForever,
                    DeleteAllAsync, Disabled: empty)
            ]
        });
    }

    private async Task ReloadAsync()
    {
        _favorites = await FavoriteService.GetAllFavoritesAsync();

        // The actions carry both the mode and whether there is anything left to act on, so they are
        // re-declared on every reload rather than only when the mode changes.
        ConfigureShell();
        StateHasChanged();
    }

    private void SetMode(ListMode mode)
    {
        _mode = mode;
        _selected.Clear();
        ConfigureShell();

        // ActionsBar owns the action's click, so its EventCallback renders that component and never
        // this page - the rows would otherwise keep the previous mode's appearance.
        StateHasChanged();
    }

    private async Task ToggleDeleteModeAsync()
    {
        // The second tap of the Delete action is what commits the selection, so the one action both
        // enters the mode and ends it.
        if (_mode == ListMode.Delete)
        {
            await DeleteSelectedAsync();
            return;
        }

        SetMode(ListMode.Delete);
        Snackbar.Add("Tap on any favorites then tap delete again to delete them", Severity.Info);
    }

    /// <summary>Leaves Delete Mode with the selection discarded and nothing deleted.</summary>
    private Task ExitDeleteModeAsync()
    {
        SetMode(ListMode.Normal);
        return Task.CompletedTask;
    }

    private async Task OnRowClick(BlueprintFavorite favorite)
    {
        if (_mode == ListMode.Normal)
        {
            await RenameAsync(favorite);
            return;
        }

        // Remove reports whether the id was selected, so the toggle costs one lookup either way.
        if (!_selected.Remove(favorite.Id))
        {
            _selected.Add(favorite.Id);
        }
    }

    private string RowClass(BlueprintFavorite favorite) =>
        _mode == ListMode.Delete && _selected.Contains(favorite.Id) ? "list-row-selected" : "";

    private async Task RenameAsync(BlueprintFavorite favorite)
    {
        DialogParameters parameters = new()
        {
            ["Label"] = "Favorite name",
            ["InitialValue"] = favorite.Name ?? ""
        };

        IDialogReference dialogRef = await DialogService.ShowAsync<TextInputDialog>("Rename Favorite", parameters);
        DialogResult? result = await dialogRef.Result;

        if (result is null or { Canceled: true } || result.Data is not string name
            || string.IsNullOrWhiteSpace(name) || name == favorite.Name)
        {
            return;
        }

        if (await FavoriteService.DoesFavoriteExistAsync(name))
        {
            Snackbar.Add($"A favorite named '{name}' already exists", Severity.Warning);
            return;
        }

        string previousName = favorite.Name ?? "";
        await FavoriteService.RenameFavoriteAsync(favorite, name);
        State.OnFavoriteRenamed(previousName, name);

        Snackbar.Add($"Renamed to '{name}'", Severity.Success);
        await ReloadAsync();
    }

    private async Task DeleteAsync(BlueprintFavorite favorite)
    {
        bool? confirmed = await DialogService.ShowMessageBoxAsync(
            "Delete favorite?",
            $"This will permanently delete '{favorite.Name}'.",
            yesText: "Delete", cancelText: "Cancel");

        if (confirmed != true)
        {
            return;
        }

        await FavoriteService.DeleteFavoriteAsync(favorite);
        State.OnFavoriteDeleted(favorite.Name ?? "");

        Snackbar.Add($"Deleted '{favorite.Name}'", Severity.Success);
        await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        List<BlueprintFavorite> selected = [.. _favorites.Where(favorite => _selected.Contains(favorite.Id))];

        // Short-circuits, so an empty selection leaves the mode without putting a confirmation for zero
        // favorites on screen.
        if (selected.Count == 0 || !await ConfirmDeleteManyAsync(selected.Count))
        {
            SetMode(ListMode.Normal);
            return;
        }

        await DeleteManyAsync(selected);

        Snackbar.Add($"Deleted {selected.Count} {NounFor(selected.Count)}", Severity.Success);
        SetMode(ListMode.Normal);
    }

    private async Task DeleteAllAsync()
    {
        if (!await ConfirmDeleteManyAsync(_favorites.Count))
        {
            return;
        }

        await DeleteManyAsync(_favorites);

        Snackbar.Add("Deleted all favorites", Severity.Success);
        SetMode(ListMode.Normal);
    }

    /// <summary>Deletes the favorites, keeping <see cref="CraftState"/> in step, then reloads the list.</summary>
    private async Task DeleteManyAsync(IReadOnlyList<BlueprintFavorite> favorites)
    {
        await FavoriteService.DeleteFavoritesAsync(favorites);

        foreach (BlueprintFavorite favorite in favorites)
        {
            State.OnFavoriteDeleted(favorite.Name ?? "");
        }

        await ReloadAsync();
    }

    private async Task<bool> ConfirmDeleteManyAsync(int count)
    {
        string noun = NounFor(count);

        bool? confirmed = await DialogService.ShowMessageBoxAsync(
            $"Delete {count} {noun}?",
            $"{count} {noun} will be deleted forever. The blueprints they hold are not affected.",
            yesText: "Delete", cancelText: "Cancel");

        return confirmed == true;
    }

    /// <summary>The noun agreeing with <paramref name="count"/>, for text that counts favorites.</summary>
    private static string NounFor(int count) => count == 1 ? "favorite" : "favorites";

    public void Dispose() => PageShellState.Reset(this);
}
