using Microsoft.AspNetCore.Components;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// The app's message dialog: a line of text and up to three buttons, in Material's order - the dismissive
/// action first, an optional alternative, and the confirming action last as the only emphasized one. Callers
/// use <see cref="ConfirmAsync"/>, <see cref="ChooseAsync"/> or <see cref="AlertAsync"/> rather than showing
/// it themselves.
/// </summary>
public partial class ConfirmDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>What the dialog asks, in one or two sentences.</summary>
    [Parameter] public string Message { get; set; } = "";

    /// <summary>The confirming button's label, naming the outcome ("Delete", "Load", "Save anyway").</summary>
    [Parameter] public string ConfirmText { get; set; } = "";

    /// <summary><see cref="Color.Error"/> when confirming destroys something, primary otherwise.</summary>
    [Parameter] public Color ConfirmColor { get; set; } = Color.Primary;

    /// <summary>The label of the second choice, shown between the dismissive and confirming buttons.</summary>
    [Parameter] public string AlternativeText { get; set; } = "";

    /// <summary>The dismissive button's label.</summary>
    [Parameter] public string CancelText { get; set; } = "Cancel";

    /// <summary>
    /// Asks <paramref name="message"/> and returns true only if the user chose <paramref name="confirmText"/>.
    /// </summary>
    public static async Task<bool> ConfirmAsync(
        IDialogService dialogs, string title, string message, string confirmText,
        Color confirmColor = Color.Primary, string cancelText = "Cancel")
    {
        DialogParameters<ConfirmDialog> parameters = new()
        {
            { dialog => dialog.Message, message },
            { dialog => dialog.ConfirmText, confirmText },
            { dialog => dialog.ConfirmColor, confirmColor },
            { dialog => dialog.CancelText, cancelText }
        };

        return await ShowAsync(dialogs, title, parameters) == true;
    }

    /// <summary>
    /// Offers two ways forward and returns true for <paramref name="confirmText"/>, false for
    /// <paramref name="alternativeText"/>, and null if the user backed out.
    /// </summary>
    public static Task<bool?> ChooseAsync(
        IDialogService dialogs, string title, string message, string confirmText, string alternativeText,
        string cancelText = "Cancel")
    {
        DialogParameters<ConfirmDialog> parameters = new()
        {
            { dialog => dialog.Message, message },
            { dialog => dialog.ConfirmText, confirmText },
            { dialog => dialog.AlternativeText, alternativeText },
            { dialog => dialog.CancelText, cancelText }
        };

        return ShowAsync(dialogs, title, parameters);
    }

    /// <summary>States <paramref name="message"/> and waits for the user to close it.</summary>
    public static Task AlertAsync(IDialogService dialogs, string title, string message)
    {
        DialogParameters<ConfirmDialog> parameters = new()
        {
            { dialog => dialog.Message, message },
            { dialog => dialog.CancelText, "Close" }
        };

        return ShowAsync(dialogs, title, parameters);
    }

    private static async Task<bool?> ShowAsync(IDialogService dialogs, string title, DialogParameters<ConfirmDialog> parameters)
    {
        IDialogReference dialogRef = await dialogs.ShowAsync<ConfirmDialog>(title, parameters);
        DialogResult? result = await dialogRef.Result;

        // Cancel, the backdrop and Escape all come back canceled; only the two buttons carry data.
        return result is null or { Canceled: true } ? null : result.Data as bool?;
    }

    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));

    private void ChooseAlternative() => MudDialog.Close(DialogResult.Ok(false));

    private void Cancel() => MudDialog.Cancel();
}
