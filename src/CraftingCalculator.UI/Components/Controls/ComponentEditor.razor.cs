using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Domain.Models;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class ComponentEditor : ComponentBase
{
    [Parameter, EditorRequired] public ComponentModel Model { get; set; } = null!;

    /// <summary>Raised on every edit, so the hosting page can track unsaved changes.</summary>
    [Parameter] public EventCallback OnChanged { get; set; }

    [Inject] private ISelectedDatasetState SelectedDataset { get; set; } = null!;

    private Task NotifyChangedAsync() => OnChanged.InvokeAsync();
}
