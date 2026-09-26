using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase
{
    /// <summary>The raw materials the whole batch needs, one entry per component.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<ComponentQuantity> Materials { get; set; } = [];

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    private List<string> DetailsFor(ComponentQuantity componentQuantity)
    {
        List<string> details = [$"Quantity: x{componentQuantity.Quantity}"];

        if (SelectedDataset.Settings.UseCosts)
        {
            details.Add($"Cost: {CurrencyProcessor.Format(componentQuantity.TotalCost, SelectedDataset.Settings)}");
        }

        return details;
    }
}
