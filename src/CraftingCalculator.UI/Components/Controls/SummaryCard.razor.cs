using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SummaryCard : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;

    private string ProfitClass => State.Profit switch
    {
        > 0 => "summary-profit summary-profit-positive",
        < 0 => "summary-profit summary-profit-negative",
        _ => "summary-profit"
    };

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
