using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SurplusList : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private List<string> DetailsFor(BlueprintQuantity blueprintQuantity)
    {
        List<string> details = [$"Quantity: x{blueprintQuantity.Quantity}"];

        if (SelectedDataset.Settings.UseValues)
        {
            details.Add($"Value: {string.Format(FormatConstants.CurrencyFormat, blueprintQuantity.TotalValue)}");
        }

        return details;
    }

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
