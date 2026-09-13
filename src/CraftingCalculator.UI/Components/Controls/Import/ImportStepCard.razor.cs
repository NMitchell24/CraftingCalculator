using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls.Import;

/// <summary>The card an import wizard step opens with: the step's title over what the step says and asks.</summary>
public partial class ImportStepCard : ComponentBase
{
    /// <summary>The step's title, such as "Step 1: Select a file".</summary>
    [Parameter, EditorRequired] public string Title { get; set; } = "";

    /// <summary>The step's text and buttons.</summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
