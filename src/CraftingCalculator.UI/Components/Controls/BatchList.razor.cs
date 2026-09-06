using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BatchList : ComponentBase, IDisposable
{
    [Inject] private CalculatorState State { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private Task ShowInfoAsync(RecipeQuantity rq) =>
        DialogService.ShowMessageBoxAsync(rq.Name, rq.Tooltip);

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
