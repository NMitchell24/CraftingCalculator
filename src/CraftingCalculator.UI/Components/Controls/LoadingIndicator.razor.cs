using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// Stands in for a screen's or a dialog's content while it is read. On a screen it fills the content area between
/// the app bar and the bars below, and the bars stay usable. It appears only once the read has taken a moment, so a
/// quick one never flashes it.
/// </summary>
public partial class LoadingIndicator : ComponentBase;
