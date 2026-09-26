using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class BatchList : ComponentBase
{
    /// <summary>The batch whose entries are listed and edited.</summary>
    [Parameter, EditorRequired] public CraftState State { get; set; } = null!;

    /// <summary>Invoked by the empty state's Add Blueprints button; opens the owner's blueprint picker.</summary>
    [Parameter] public EventCallback OnAddBlueprints { get; set; }

    private static string RemoveLabel(BlueprintQuantity selected) => $"Remove {selected.Name} from the batch";
}
