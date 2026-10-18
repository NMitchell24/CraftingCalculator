using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.Components.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase
{
    // The pitch of a component's row at 100% text with both detail lines (quantity and cost): the card's 100 px plus
    // the 6 px margin-bottom app.css gives every card in a list. Virtualize starts from this and re-measures from the
    // rows it has rendered, so a missing cost line, an already crafted blueprint's third line or a name that wraps
    // only makes the scrollbar approximate.
    private const float RowHeight = 106;

    /// <summary>
    /// Everything the whole batch gathers rather than crafts, one entry per component and per already crafted
    /// blueprint.
    /// </summary>
    [Parameter, EditorRequired]
    public IReadOnlyList<IBaseQuantityRecord> Materials { get; set; } = [];

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    private List<string> DetailsFor(IBaseQuantityRecord quantity)
    {
        List<string> details = [$"Quantity: x{quantity.Quantity}"];

        if (quantity is BlueprintQuantity)
        {
            details.Add("Already crafted");
        }

        if (SelectedDataset.Settings.UseCosts)
        {
            details.Add($"Cost: {CurrencyProcessor.Format(BatchProcessor.CostOf(quantity, SelectedDataset.Settings), SelectedDataset.Settings)}");
        }

        return details;
    }

    // A component opens the card's own Info dialog; an already crafted blueprint opens one that says so.
    private EventCallback InfoFor(IBaseQuantityRecord quantity) => quantity is BlueprintQuantity blueprintQuantity
        ? EventCallback.Factory.Create(this, () => InfoDialog.ShowAlreadyCraftedAsync(DialogService, blueprintQuantity.Blueprint))
        : default;
}
