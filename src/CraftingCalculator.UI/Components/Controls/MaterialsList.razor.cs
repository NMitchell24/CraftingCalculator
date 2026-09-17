using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private static List<string> DetailsFor(ComponentQuantity componentQuantity) =>
    [
        $"Quantity: x{componentQuantity.Quantity}",
        $"Cost: {string.Format(FormatConstants.CurrencyFormat, componentQuantity.TotalCost)}"
    ];

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
