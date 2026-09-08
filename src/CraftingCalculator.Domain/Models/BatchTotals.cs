namespace CraftingCalculator.Domain.Models;

/// <summary>
/// What a batch of blueprints costs, is worth, consumes, overproduces, and takes to make.
/// <see cref="Surplus"/> is the items produced beyond what the batch asked for, because a craft is
/// indivisible: its value is reported separately and is not part of <see cref="TotalValue"/>.
/// <see cref="TotalProductionTime"/> covers both the blueprint crafts and the components they consume,
/// summed as though the batch were made one step at a time.
/// </summary>
public sealed record BatchTotals(
    double TotalCost,
    double TotalValue,
    ComponentMap Materials,
    BlueprintMap Surplus,
    TimeSpan TotalProductionTime);
