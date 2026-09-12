using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The prompts the Dataset screen runs: the delete confirmations for one record or several - shared by
/// the Dataset list's row actions, its Delete Mode, and the editor's action bar - and the naming and
/// delete sequences for the datasets themselves.
/// </summary>
public static class DatasetPrompts
{
    /// <summary>
    /// Asks for a dataset name, re-asking until it is one no other dataset uses or the user backs out.
    /// Returns the name, or null if they did. <paramref name="exceptId"/> is the dataset being renamed,
    /// so keeping its own name is not reported as a collision.
    /// </summary>
    public static async Task<string?> PromptForDatasetNameAsync(
        IDialogService dialogs, IDatasetService datasets, string title, string initialValue = "", int exceptId = 0)
    {
        string entered = initialValue;

        while (true)
        {
            DialogParameters parameters = new() { ["Label"] = "Dataset name", ["InitialValue"] = entered };
            IDialogReference dialogRef = await dialogs.ShowAsync<TextInputDialog>(title, parameters);
            DialogResult? result = await dialogRef.Result;

            if (result is null or { Canceled: true } || result.Data is not string name || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (!await datasets.NameExistsAsync(name, exceptId))
            {
                return name;
            }

            // A loop rather than FavoritePrompts' overwrite branch: merging two datasets into one is not
            // something a name collision should be allowed to mean. The rejected name is carried back
            // into the field so the user edits it rather than retyping it.
            entered = name;

            await dialogs.ShowMessageBoxAsync(
                "Name already in use",
                $"A dataset named '{name}' already exists. Pick a different name.",
                yesText: "OK");
        }
    }

    /// <summary>Returns true only if the user confirmed deleting the dataset and everything in it.</summary>
    public static async Task<bool> ConfirmDeleteDatasetAsync(IDialogService dialogs, DatasetModel dataset)
    {
        DialogParameters parameters = new()
        {
            ["Message"] = $"'{dataset.Name}' and every category, component, blueprint and favorite in it "
                          + "will be deleted forever. Your other datasets are not affected.",
            ["ConfirmWord"] = "DELETE",
            ["ConfirmText"] = "Delete dataset"
        };

        IDialogReference dialogRef = await dialogs.ShowAsync<TypedConfirmDialog>($"Delete {dataset.Name}?", parameters);
        DialogResult? result = await dialogRef.Result;

        return result is not (null or { Canceled: true });
    }

    /// <summary>Returns true only if the user confirmed the delete.</summary>
    public static async Task<bool> ConfirmDeleteAsync(IDialogService dialogs, IBaseDataRecord record)
    {
        // Ports the warning from ConfigureBlueprintsViewModel.DeleteItem: a deleted record disappears
        // from every blueprint that used it, and a deleted blueprint from every favorite as well.
        string alsoFavorites = record.Type == DataType.Blueprint ? "or blueprint favorites " : "";

        bool? confirmed = await dialogs.ShowMessageBoxAsync(
            $"Delete {record.Type.GetDescription()}?",
            $"'{record.Name}' will be deleted forever and removed from any blueprints {alsoFavorites}where it is used.",
            yesText: "Delete", cancelText: "Cancel");

        return confirmed == true;
    }

    /// <summary>
    /// Confirms deleting <paramref name="count"/> records of <paramref name="type"/>, named by
    /// <paramref name="noun"/>, which the caller supplies already agreeing with <paramref name="count"/>.
    /// Returns true only if the user confirmed.
    /// </summary>
    public static async Task<bool> ConfirmDeleteManyAsync(IDialogService dialogs, DataType type, string noun, int count)
    {
        string alsoFavorites = type == DataType.Blueprint ? "or blueprint favorites " : "";

        bool? confirmed = await dialogs.ShowMessageBoxAsync(
            $"Delete {count} {noun}?",
            $"{count} {noun} will be deleted forever and removed from any blueprints {alsoFavorites}where they are used.",
            yesText: "Delete", cancelText: "Cancel");

        return confirmed == true;
    }
}
