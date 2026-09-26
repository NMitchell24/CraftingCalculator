using System.Collections.ObjectModel;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SurplusList : ComponentBase
{
    // The pitch of a row at 100% text with both detail lines (quantity and value): the card's 100 px plus the 6 px
    // margin-bottom app.css gives every card in a list. Virtualize starts from this and re-measures from the rows it
    // has rendered, so a missing value line or a name that wraps only makes the scrollbar approximate.
    private const float RowHeight = 106;

    /// <summary>The blueprints the batch overproduces, with how many are left over.</summary>
    [Parameter, EditorRequired]
    public ReadOnlyCollection<BlueprintQuantity> Surplus { get; set; } = ReadOnlyCollection<BlueprintQuantity>.Empty;

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
