namespace CraftingCalculator.Domain.Models;

/// <summary>
/// One node in a blueprint's component breakdown tree: either the blueprint itself, one of its nested
/// child blueprints, or a leaf component. <see cref="Id"/> matches the source blueprint/component's
/// <c>Name</c>, not its numeric id - same-named blueprints collide, matching the WPF app's tree.
/// <see cref="Quantity"/> is the effective amount at this position in the tree, already multiplied
/// through every ancestor's quantity.
/// </summary>
public sealed record BlueprintNode(
    string Name,
    string? Id,
    string Tooltip,
    bool IsComponent,
    long Quantity,
    IReadOnlyList<BlueprintNode> Children);
