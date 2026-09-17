namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// The categories, components and blueprints of one dataset as flat, id-keyed rows, each list in id order:
/// everything a <see cref="DatasetSnapshot"/> holds besides its name and favorites. Every id a link names
/// belongs to a record in the same set.
/// </summary>
public sealed record DatasetRecords(
    IReadOnlyList<SnapshotCategory> Categories,
    IReadOnlyList<SnapshotComponent> Components,
    IReadOnlyList<SnapshotBlueprint> Blueprints);
