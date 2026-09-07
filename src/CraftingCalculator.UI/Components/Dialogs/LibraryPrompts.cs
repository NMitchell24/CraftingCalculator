using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The delete confirmation for a single Library record, shared by the Library list's row menu and the
/// editor's app-bar menu.
/// </summary>
public static class LibraryPrompts
{
    /// <summary>Returns true only if the user confirmed the delete.</summary>
    public static async Task<bool> ConfirmDeleteAsync(IDialogService dialogs, IBaseDataRecord record)
    {
        // Ports the warning from ConfigureRecipesViewModel.DeleteItem: a deleted record disappears
        // from every recipe that used it, and a deleted recipe from every favorite as well.
        string alsoFavorites = record.Type == DataType.Recipe ? "or blueprint favorites " : "";

        bool? confirmed = await dialogs.ShowMessageBoxAsync(
            $"Delete {record.Type.GetDescription()}?",
            $"'{record.Name}' will be deleted forever and removed from any blueprints {alsoFavorites}where it is used.",
            yesText: "Delete", cancelText: "Cancel");

        return confirmed == true;
    }
}
