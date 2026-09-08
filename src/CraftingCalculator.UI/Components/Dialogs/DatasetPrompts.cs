using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The delete confirmations for Dataset records - one record or several - shared by the Dataset list's
/// row actions, its Delete Mode, and the editor's action bar.
/// </summary>
public static class DatasetPrompts
{
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
