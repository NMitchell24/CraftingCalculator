using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Pages;

/// <summary>
/// The saved favorites. Each favorite is a card whose buttons show, rename or delete it; while the list is in
/// Delete mode (<see cref="ListMode"/>), a tap on a card selects it.
/// </summary>
public partial class Favorites : ComponentBase, IDisposable
{
    /// <summary>What a card tap does, driven by the Delete action on the shell.</summary>
    private enum ListMode
    {
        /// <summary>A card tap does nothing.</summary>
        Normal,

        /// <summary>Card taps toggle selection; the Delete action then deletes the selection.</summary>
        Delete
    }

    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;

    // One message for all three delete paths, so it says nothing about how many favorites were involved: the
    // list behind the dialog still shows them either way.
    private const string DeleteFailedMessage = "I couldn't finish that delete.";

    private List<BlueprintFavorite> _favorites = [];
    private ListMode _mode = ListMode.Normal;

    // Ids rather than favorites: ReloadAsync replaces every instance, and these models have no value
    // equality, so a selection held as favorites would not survive a reload.
    private readonly HashSet<int> _selected = [];

    protected override async Task OnInitializedAsync()
    {
        // Declared before anything is awaited: until the favorites arrive the shell would otherwise
        // still carry the outgoing page's title and actions, whose callbacks belong to that component.
        // The empty list makes both actions disabled, which is what an empty Favorites screen shows
        // anyway; ReloadAsync re-declares them against the loaded list.
        ConfigureShell();

        await ReloadAsync();
    }

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

    private void ToggleSelection(BlueprintFavorite favorite)
    {
        if (_mode != ListMode.Delete)
        {
            return;
        }

        // Remove reports whether the id was selected, so the toggle costs one lookup either way.
        if (!_selected.Remove(favorite.Id))
        {
            _selected.Add(favorite.Id);
        }
    }

    private string RowClass(BlueprintFavorite favorite) =>
        _mode == ListMode.Delete && _selected.Contains(favorite.Id) ? "list-item-selected" : "";

    private static IReadOnlyList<string> DetailsFor(BlueprintFavorite favorite) =>
        [$"{favorite.BlueprintCount} blueprint{(favorite.BlueprintCount == 1 ? "" : "s")}"];

    private async Task ShowInfoAsync(BlueprintFavorite favorite) =>
        await InfoDialog.ShowAsync(
            DialogService, favorite, await FavoriteService.GetBlueprintQuantitiesForFavoriteAsync(favorite));

    private async Task RenameAsync(BlueprintFavorite favorite)
    {
        if (await FavoritePrompts.PromptForRenameAsync(DialogService, FavoriteService, favorite) is not { } name)
        {
            return;
        }

        bool renamed = await Guard.RunAsync(
            "Favorites.Rename",
            "I couldn't rename that favorite. It still has its old name.",
            async () =>
            {
                await FavoriteService.RenameFavoriteAsync(favorite, name);
                State.OnFavoriteRenamed(favorite.Id, name);

                await ReloadAsync();
            });

        if (renamed)
        {
            Snackbar.Add($"Renamed to '{name}'", Severity.Success);
        }
    }

    private async Task DeleteAsync(BlueprintFavorite favorite)
    {
        bool confirmed = await ConfirmDialog.ConfirmAsync(
            DialogService,
            "Delete favorite?",
            $"This will permanently delete '{favorite.Name}'.",
            "Delete", Color.Error);

        if (!confirmed)
        {
            return;
        }

        bool deleted = await Guard.RunAsync(
            "Favorites.Delete",
            DeleteFailedMessage,
            async () =>
            {
                await FavoriteService.DeleteFavoriteAsync(favorite);
                State.OnFavoriteDeleted(favorite.Id);

                await ReloadAsync();
            });

        if (deleted)
        {
            Snackbar.Add($"Deleted '{favorite.Name}'", Severity.Success);
        }
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

        bool deleted = await Guard.RunAsync(
            "Favorites.DeleteSelected", DeleteFailedMessage, () => DeleteManyAsync(selected));

        if (deleted)
        {
            Snackbar.Add($"Deleted {selected.Count} {NounFor(selected.Count)}", Severity.Success);
        }

        // Left whichever way it went: the selection is either deleted or reported as failed, and keeping the
        // mode on would leave the user staring at highlighted cards with a dialog telling them nothing happened.
        SetMode(ListMode.Normal);
    }

    private async Task DeleteAllAsync()
    {
        if (!await ConfirmDeleteManyAsync(_favorites.Count))
        {
            return;
        }

        bool deleted = await Guard.RunAsync(
            "Favorites.DeleteAll", DeleteFailedMessage, () => DeleteManyAsync(_favorites));

        if (deleted)
        {
            Snackbar.Add("Deleted all favorites", Severity.Success);
        }

        SetMode(ListMode.Normal);
    }

    /// <summary>Deletes the favorites, keeping <see cref="CraftState"/> in step, then reloads the list.</summary>
    private async Task DeleteManyAsync(IReadOnlyList<BlueprintFavorite> favorites)
    {
        await FavoriteService.DeleteFavoritesAsync(favorites);

        foreach (BlueprintFavorite favorite in favorites)
        {
            State.OnFavoriteDeleted(favorite.Id);
        }

        await ReloadAsync();
    }

    private Task<bool> ConfirmDeleteManyAsync(int count)
    {
        string noun = NounFor(count);

        return ConfirmDialog.ConfirmAsync(
            DialogService,
            $"Delete {count} {noun}?",
            $"{count} {noun} will be deleted forever. The blueprints they hold are not affected.",
            "Delete", Color.Error);
    }

    /// <summary>The noun agreeing with <paramref name="count"/>, for text that counts favorites.</summary>
    private static string NounFor(int count) => count == 1 ? "favorite" : "favorites";

    public void Dispose() => PageShellState.Reset(this);
}
