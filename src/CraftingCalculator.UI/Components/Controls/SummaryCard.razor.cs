using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SummaryCard : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    private bool UseCosts => SelectedDataset.Settings.UseCosts;
    private bool UseValues => SelectedDataset.Settings.UseValues;

    private string ProfitClass => State.Profit switch
    {
        > 0 => "summary-profit-positive",
        < 0 => "summary-profit-negative",
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
