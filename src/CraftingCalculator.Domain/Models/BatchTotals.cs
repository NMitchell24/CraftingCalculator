namespace CraftingCalculator.Domain.Models;

/// <summary>
/// What a batch of blueprints costs, is worth, consumes, and overproduces. <see cref="Surplus"/> is the
/// items produced beyond what the batch asked for, because a craft is indivisible: its value is reported
/// separately and is not part of <see cref="TotalValue"/>.
/// </summary>
public sealed record BatchTotals(
    double TotalCost,
    double TotalValue,
    ComponentMap Materials,
    BlueprintMap Surplus);
