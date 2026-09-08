namespace CraftingCalculator.Domain.Models;

/// <summary>
/// What one blueprint needs and produces at a given quantity: the raw <see cref="Components"/> left
/// after every nested blueprint is resolved, the items its indivisible crafts overproduce, and how long
/// those crafts take. <see cref="ProductionTime"/> covers blueprint crafts only; the time the components
/// themselves take is added where the materials are totalled, since it follows the merged quantities.
/// </summary>
public sealed record FlattenResult(
    ComponentMap Components,
    BlueprintMap Surplus,
    TimeSpan ProductionTime);
