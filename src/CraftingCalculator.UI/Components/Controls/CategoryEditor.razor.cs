using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class CategoryEditor : ComponentBase
{
    [Parameter, EditorRequired] public RecipeFilter Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
