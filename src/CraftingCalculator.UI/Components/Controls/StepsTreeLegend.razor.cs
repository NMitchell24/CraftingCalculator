using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

// Its own component so a single row's expand arrow re-renders only this, not the whole StepsTree: the legend is the
// one thing outside the toggled row that can change.
public partial class StepsTreeLegend : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.HasVisibleCraftCountedStepChanged += StateHasChanged;
    }

    public void Dispose()
    {
        State.HasVisibleCraftCountedStepChanged -= StateHasChanged;
    }
}
