using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The prompt sequences for saving the current batch as a favorite and for loading one back into it,
/// shared by the Craft and Favorites screens.
/// </summary>
public static class FavoritePrompts
{
    /// <summary>
    /// Confirms discarding the current batch when there is one, then loads <paramref name="favorite"/>
    /// into <paramref name="state"/>. Returns true when the batch was replaced.
    /// </summary>
    public static async Task<bool> LoadAsync(IDialogService dialogs, ISnackbar snackbar, CraftState state, BlueprintFavorite favorite)
    {
        // WPF replaced the working batch silently (BlueprintsViewModel.SelectedFav); on a phone that is
        // one mis-tap from discarding unsaved work.
        if (state.BlueprintQuantities.Count > 0)
        {
            bool? replace = await dialogs.ShowMessageBoxAsync(
                "Replace current selection?",
                $"Loading '{favorite.Name}' will discard the blueprints you have selected.",
                yesText: "Load", cancelText: "Cancel");

            if (replace != true)
            {
                return false;
            }
        }

        await state.LoadFavoriteAsync(favorite);
        snackbar.Add($"Loaded '{favorite.Name}'", Severity.Success);

        return true;
    }

    /// <summary>
    /// Runs the save sequence and returns the name the batch was saved under, or null if the batch is
    /// empty or the user backed out at any step.
    /// </summary>
    public static async Task<string?> SaveBatchAsync(IDialogService dialogs, ISnackbar snackbar, CraftState state)
    {
        if (state.BlueprintQuantities.Count == 0)
        {
            return null;
        }

        string? name = null;

        if (state.LoadedFavoriteName is { } loaded)
        {
            bool? update = await dialogs.ShowMessageBoxAsync(
                "Update or Create New?",
                $"Would you like to update '{loaded}' or create a new favorite?",
                yesText: "Update", noText: "Create New", cancelText: "Cancel");

            if (update is null)
            {
                return null;
            }

            if (update is true)
            {
                name = loaded;
            }
        }

        name ??= await PromptForNewNameAsync(dialogs, state);

        if (name is null)
        {
            return null;
        }

        await state.SaveAsFavoriteAsync(name);
        snackbar.Add($"Saved '{name}'", Severity.Success);

        return name;
    }

    /// <summary>
    /// Asks for a name for a new favorite, confirming the overwrite when one of that name already
    /// exists. Returns the name to save under, or null if the user backed out at either step.
    /// </summary>
    private static async Task<string?> PromptForNewNameAsync(IDialogService dialogs, CraftState state)
    {
        DialogParameters parameters = new() { ["Label"] = "Favorite name" };
        IDialogReference dialogRef = await dialogs.ShowAsync<TextInputDialog>("Save as Favorite", parameters);
        DialogResult? result = await dialogRef.Result;

        if (result is null or { Canceled: true } || result.Data is not string entered || string.IsNullOrWhiteSpace(entered))
        {
            return null;
        }

        // Only this path checks for a collision - updating the loaded favorite is already an overwrite
        // by definition. Mirrors BlueprintsViewModel.SaveBlueprints.
        if (!await state.FavoriteExistsAsync(entered))
        {
            return entered;
        }

        bool? overwrite = await dialogs.ShowMessageBoxAsync(
            "Overwrite?", $"A favorite named '{entered}' already exists. Overwrite it?",
            yesText: "Overwrite", cancelText: "Cancel");

        return overwrite == true ? entered : null;
    }
}
