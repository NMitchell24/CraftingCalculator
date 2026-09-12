using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BatchList : ComponentBase, IDisposable
{
    /// <summary>Invoked by the empty state's Add Blueprints button; opens the owner's blueprint picker.</summary>
    [Parameter] public EventCallback OnAddBlueprints { get; set; }

    /// <summary>How far one tap of a row's + or - moves that blueprint's quantity.</summary>
    [Parameter] public long StepSize { get; set; } = 1;

    private bool IsBulkStep => StepSize > 1;

    private string DecrementIcon => IsBulkStep ? Icons.Material.Filled.KeyboardDoubleArrowDown : Icons.Material.Filled.Remove;

    private string IncrementIcon => IsBulkStep ? Icons.Material.Filled.KeyboardDoubleArrowUp : Icons.Material.Filled.Add;

    // The stepper is what the mode actually changes, so it is also what announces the mode is on - the
    // actions bar's own highlight is off at the other end of the screen while the user is tapping here.
    private Color StepColor => IsBulkStep ? Color.Primary : Color.Default;

    private string DecrementLabel => $"Subtract {StepSize}";

    private string IncrementLabel => $"Add {StepSize}";

    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private Task ShowInfoAsync(BlueprintQuantity selected) =>
        InfoDialog.ShowAsync(DialogService, selected.Blueprint);

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
