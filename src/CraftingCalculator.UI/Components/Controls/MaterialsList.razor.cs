using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Controls;

public partial class MaterialsList : ComponentBase, IDisposable
{
    [Inject] private CalculatorState State { get; set; } = null!;
    [Inject] private IClipboardService ClipboardService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.Changed += StateHasChanged;
    }

    private async Task CopyAllAsync()
    {
        string text = string.Join(Environment.NewLine, State.TotalIngredients.Select(i => i.DisplayName));
        await ClipboardService.SetTextAsync(text);
        Snackbar.Add("Copied materials to clipboard", Severity.Success);
    }

    public void Dispose()
    {
        State.Changed -= StateHasChanged;
    }
}
