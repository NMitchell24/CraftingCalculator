using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

/// <summary>
/// Confirms an irreversible action by making the user type <see cref="ConfirmWord"/> exactly. Closes
/// with <c>DialogResult.Ok(true)</c> on confirm.
/// </summary>
public partial class TypedConfirmDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string Message { get; set; } = "";
    [Parameter] public string ConfirmWord { get; set; } = "DELETE";
    [Parameter] public string ConfirmText { get; set; } = "Delete";

    private string _value = "";

    private void Confirm() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();
}
