using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;
    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private List<string> DetailsFor(ComponentQuantity componentQuantity)
    {
        List<string> details = [$"Quantity: x{componentQuantity.Quantity}"];

        if (SelectedDataset.Settings.UseCosts)
        {
            details.Add($"Cost: {CurrencyProcessor.Format(componentQuantity.TotalCost, SelectedDataset.Settings)}");
        }

        return details;
    }

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
