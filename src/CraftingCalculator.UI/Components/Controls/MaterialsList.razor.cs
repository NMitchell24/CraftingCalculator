using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private Task ShowInfoAsync(ComponentQuantity componentQuantity) =>
        InfoDialog.ShowAsync(DialogService, componentQuantity.Component);

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
