using System.Collections.ObjectModel;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase
{
    // The pitch of a row at 100% text with both detail lines (quantity and cost): the card's 100 px plus the 6 px
    // margin-bottom app.css gives every card in a list. Virtualize starts from this and re-measures from the rows it
    // has rendered, so a missing cost line or a name that wraps only makes the scrollbar approximate.
    private const float RowHeight = 106;

    /// <summary>The raw materials the whole batch needs, one entry per component.</summary>
    [Parameter, EditorRequired]
    public ReadOnlyCollection<ComponentQuantity> Materials { get; set; } = ReadOnlyCollection<ComponentQuantity>.Empty;

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
