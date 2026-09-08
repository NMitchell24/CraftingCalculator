namespace CraftingCalculator.Domain.Models;

/// <summary>
/// One node in a blueprint's component breakdown tree: either the blueprint itself, one of its nested
/// child blueprints, or a leaf component. <see cref="Id"/> matches the source blueprint/component's
/// <c>Name</c>, not its numeric id - same-named blueprints collide, matching the WPF app's tree.
/// <see cref="Quantity"/> is the effective amount at this position in the tree, already multiplied
/// through every ancestor's quantity. <see cref="Crafts"/> is how many craft operations produce that
/// amount, which is fewer than <see cref="Quantity"/> whenever the blueprint's yield is above 1; it is
/// 0 for a component leaf, which is gathered rather than crafted. <see cref="Yield"/> is what one of
/// those crafts produces, and <see cref="Surplus"/> is what they overproduce at this position; both are
/// likewise 0 for a component leaf.
/// <see cref="ProductionTime"/> is this row's own time - the blueprint's time across its
/// <see cref="Crafts"/>, or the component's across its <see cref="Quantity"/> - and excludes the
/// children beneath it, so summing the whole tree counts every step exactly once.
/// </summary>
public sealed record BlueprintNode(
    string Name,
    string? Id,
    string Tooltip,
    bool IsComponent,
    long Quantity,
    long Crafts,
    long Yield,
    long Surplus,
    TimeSpan ProductionTime,
    IReadOnlyList<BlueprintNode> Children);
