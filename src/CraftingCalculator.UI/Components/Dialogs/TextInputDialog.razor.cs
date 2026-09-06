using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Dialogs;

public partial class TextInputDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public string Label { get; set; } = "Name";
    [Parameter] public string InitialValue { get; set; } = "";

    private string _value = "";

    protected override void OnInitialized()
    {
        _value = InitialValue;
    }

    private void Submit() => MudDialog.Close(DialogResult.Ok(_value.Trim()));

    private void Cancel() => MudDialog.Cancel();
}
