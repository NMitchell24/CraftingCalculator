using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SurplusList : ComponentBase
{
    /// <summary>The blueprints the batch overproduces, with how many are left over.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<BlueprintQuantity> Surplus { get; set; } = [];

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    private List<string> DetailsFor(BlueprintQuantity blueprintQuantity)
    {
        List<string> details = [$"Quantity: x{blueprintQuantity.Quantity}"];

        if (SelectedDataset.Settings.UseValues)
        {
            details.Add($"Value: {CurrencyProcessor.Format(blueprintQuantity.TotalValue, SelectedDataset.Settings)}");
        }

        return details;
    }
}
