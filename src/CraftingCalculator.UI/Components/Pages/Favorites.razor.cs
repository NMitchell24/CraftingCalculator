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
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private List<BlueprintFavorite> _favorites = [];

    // Declares no actions: this page manages saved favorites, and both of its operations are per-row.
    // Saving the current batch as a favorite belongs to the Craft screen, which owns the batch.
    protected override async Task OnInitializedAsync()
    {
        PageShellState.Configure(this, new PageShellConfig("Favorites"));
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        _favorites = await FavoriteService.GetAllFavoritesAsync();
        StateHasChanged();
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

    public void Dispose() => PageShellState.Reset(this);
}
