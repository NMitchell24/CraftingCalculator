using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class StepsTree : ComponentBase, IDisposable
{
    /// <summary>The batch's crafting trees, one root per batch entry.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<BlueprintNode> Roots { get; set; } = [];

    [Inject] private CraftState State { get; set; } = null!;

    protected override void OnInitialized()
    {
        State.ExpansionChanged += StateHasChanged;
    }

    public void Dispose()
    {
        State.ExpansionChanged -= StateHasChanged;
    }
}
