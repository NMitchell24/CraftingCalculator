using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BatchList : ComponentBase, IDisposable
{
    /// <summary>Invoked by the empty state's Add Blueprints button; opens the owner's blueprint picker.</summary>
    [Parameter] public EventCallback OnAddBlueprints { get; set; }

    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private Task ShowInfoAsync(BlueprintQuantity blueprintQuantity) =>
        DialogService.ShowMessageBoxAsync(blueprintQuantity.Name, blueprintQuantity.Tooltip);

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
