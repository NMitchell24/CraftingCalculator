using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The prompt sequences for saving the current batch as a favorite and for loading one back into it,
/// used by the Craft screen.
/// </summary>
public static class FavoritePrompts
{
    /// <summary>
    /// Confirms discarding the current batch when there is one, then loads <paramref name="favorite"/>
    /// into <paramref name="state"/>.
    /// </summary>
    public static async Task LoadAsync(IDialogService dialogs, ISnackbar snackbar, CraftState state, BlueprintFavorite favorite)
    {
        // WPF replaced the working batch silently (BlueprintsViewModel.SelectedFav); on a phone that is
        // one mis-tap from discarding unsaved work.
        if (state.BlueprintQuantities.Count > 0)
        {
            bool replace = await ConfirmDialog.ConfirmAsync(
                dialogs,
                "Replace current selection?",
                $"Loading '{favorite.Name}' will discard the blueprints you have selected.",
                "Load");

            if (!replace)
            {
                return;
            }
        }

        await state.LoadFavoriteAsync(favorite);
        snackbar.Add($"Loaded '{favorite.Name}'", Severity.Success);
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
            bool? update = await ConfirmDialog.ChooseAsync(
                dialogs,
                "Update or create new?",
                $"Would you like to update '{loaded}' or create a new favorite?",
                confirmText: "Update", alternativeText: "Create new");

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
        DialogParameters parameters = new() { ["Label"] = "Favorite name", ["ConfirmText"] = "Save" };
        IDialogReference dialogRef = await dialogs.ShowAsync<TextInputDialog>("Save as favorite", parameters);
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

        bool overwrite = await ConfirmDialog.ConfirmAsync(
            dialogs,
            "Overwrite?",
            $"A favorite named '{entered}' already exists. Overwrite it?",
            "Overwrite", Color.Error);

        return overwrite ? entered : null;
    }
}
