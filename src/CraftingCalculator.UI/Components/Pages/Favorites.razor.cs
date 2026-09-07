using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Favorites : ComponentBase, IDisposable
{
    [Inject] private IFavoriteService FavoriteService { get; set; } = null!;
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private List<BlueprintFavorite> _favorites = [];

    // Unlike the Craft screen's panes, this page does not subscribe to CraftState.Changed:
    // the only thing that mutates the batch while it is on screen is its own LoadAsync, which
    // navigates away to "/" immediately afterwards.
    protected override async Task OnInitializedAsync()
    {
        AppBarState.Configure(this, new AppBarConfig("Favorites")
        {
            // The batch cannot change while this page is on screen - the only thing that mutates it is
            // LoadAsync, which navigates away - so this snapshot stays accurate for the page's life.
            PrimaryAction = new AppBarAction("Save current selection", Icons.Material.Filled.Save,
                SaveCurrentBatchAsync, Disabled: State.BlueprintQuantities.Count == 0)
        });
        await ReloadAsync();
    }

    private async Task ReloadAsync() => _favorites = await FavoriteService.GetAllFavoritesAsync();

    private async Task LoadAsync(BlueprintFavorite favorite)
    {
        // WPF replaced the working batch silently (BlueprintsViewModel.SelectedFav); on a phone that is
        // one mis-tap from discarding unsaved work.
        if (State.BlueprintQuantities.Count > 0)
        {
            bool? replace = await DialogService.ShowMessageBoxAsync(
                "Replace current selection?",
                $"Loading '{favorite.Name}' will discard the blueprints you have selected.",
                yesText: "Load", cancelText: "Cancel");

            if (replace != true)
            {
                return;
            }
        }

        await State.LoadFavoriteAsync(favorite);
        Snackbar.Add($"Loaded '{favorite.Name}'", Severity.Success);
        Navigation.NavigateTo("/");
    }

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

    private async Task SaveCurrentBatchAsync()
    {
        if (await FavoritePrompts.SaveBatchAsync(DialogService, Snackbar, State) is not null)
        {
            await ReloadAsync();
        }
    }

    public void Dispose() => AppBarState.Reset(this);
}
