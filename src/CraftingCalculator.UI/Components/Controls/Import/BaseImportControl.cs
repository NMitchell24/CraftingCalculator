using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls.Import;

/// <summary>
/// A step of the import wizard, which shows and advances the wizard in <see cref="State.ImportState"/>. The Import page
/// renders one step control per <see cref="ImportStep"/>, so a step control lives exactly as long as the wizard stays on
/// its step.
/// </summary>
public abstract class BaseImportControl : ComponentBase
{
    [Inject] protected ImportState ImportState { get; set; } = null!;
}
