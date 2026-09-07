using CraftingCalculator.Domain.Models;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Controls;

public partial class RecipeTreeNode : ComponentBase
{
    [Parameter, EditorRequired] public RecipeNode Node { get; set; } = null!;

    /// <summary>The path of the parent node, empty for a tree root. See <see cref="Path"/>.</summary>
    [Parameter] public string ParentPath { get; set; } = "";

    [Inject] private CalculatorState State { get; set; } = null!;

    /// <summary>
    /// This node's position in the tree, e.g. "/Frame/Bracket" - distinct from <see cref="Node"/>.Id
    /// (which is just this node's own name) so that CalculatorState can track expansion per tree
    /// position rather than per recipe/ingredient name. The same recipe can appear more than once in
    /// one tree (standalone in the batch and nested inside another batch recipe); keying by Id alone
    /// would make every occurrence share one expanded/collapsed state.
    /// </summary>
    private string Path => $"{ParentPath}/{Node.Id ?? Node.Name}";
}
