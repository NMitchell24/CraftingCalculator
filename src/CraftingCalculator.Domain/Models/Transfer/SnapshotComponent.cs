namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// One component in a <see cref="DatasetSnapshot"/>. <see cref="CategoryId"/> names a
/// <see cref="SnapshotCategory"/> in the same snapshot, or is null for an uncategorized component.
/// </summary>
public sealed record SnapshotComponent(
    int Id, string Name, string Description, double Cost, TimeSpan ProductionTime, int? CategoryId);
