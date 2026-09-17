using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class SurplusList : ComponentBase, IDisposable
{
    [Inject] private CraftState State { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private static List<string> DetailsFor(BlueprintQuantity blueprintQuantity) =>
    [
        $"Quantity: x{blueprintQuantity.Quantity}",
        $"Value: {string.Format(FormatConstants.CurrencyFormat, blueprintQuantity.TotalValue)}"
    ];

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
