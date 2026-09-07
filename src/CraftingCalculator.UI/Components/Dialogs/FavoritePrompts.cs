using CraftingCalculator.UI.State;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The prompt sequence for saving the current batch as a favorite, shared by the Calculate screen's
/// app-bar menu and the Favorites screen's FAB.
/// </summary>
public static class FavoritePrompts
{
    /// <summary>
    /// Runs the save sequence and returns the name the batch was saved under, or null if the batch is
    /// empty or the user backed out at any step.
    /// </summary>
    public static async Task<string?> SaveBatchAsync(IDialogService dialogs, ISnackbar snackbar, CalculatorState state)
    {
        if (state.RecipeQuantities.Count == 0)
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

        // Only the create-new path checks for a name collision - updating the loaded favorite is
        // already an overwrite by definition. Mirrors RecipesViewModel.SaveRecipes.
        if (name is null)
        {
            DialogParameters parameters = new() { ["Label"] = "Favorite name" };
            IDialogReference dialogRef = await dialogs.ShowAsync<TextInputDialog>("Save as Favorite", parameters);
            DialogResult? result = await dialogRef.Result;

            if (result is null or { Canceled: true } || result.Data is not string entered || string.IsNullOrWhiteSpace(entered))
            {
                return null;
            }

            name = entered;

            if (await state.FavoriteExistsAsync(name))
            {
                bool? overwrite = await dialogs.ShowMessageBoxAsync(
                    "Overwrite?", $"A favorite named '{name}' already exists. Overwrite it?",
                    yesText: "Overwrite", cancelText: "Cancel");

                if (overwrite != true)
                {
                    return null;
                }
            }
        }

        await state.SaveAsFavoriteAsync(name);
        snackbar.Add($"Saved '{name}'", Severity.Success);

        return name;
    }
}
