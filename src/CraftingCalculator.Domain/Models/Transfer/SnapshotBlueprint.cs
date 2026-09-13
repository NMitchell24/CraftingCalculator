namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// One blueprint in a <see cref="DatasetSnapshot"/>. <see cref="Components"/> link to
/// <see cref="SnapshotComponent"/>s and <see cref="Blueprints"/> to the child <see cref="SnapshotBlueprint"/>s
/// nested inside it, all in the same snapshot.
/// </summary>
public sealed record SnapshotBlueprint(
    int Id,
    string Name,
    string Description,
    double Value,
    long Yield,
    TimeSpan ProductionTime,
    int? CategoryId,
    IReadOnlyList<QuantityLink> Components,
    IReadOnlyList<QuantityLink> Blueprints);
