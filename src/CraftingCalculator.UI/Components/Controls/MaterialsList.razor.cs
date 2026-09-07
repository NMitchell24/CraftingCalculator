using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase, IDisposable
{
    [Inject] private CalculatorState State { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
