namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// Every record of one dataset as flat, id-keyed rows, whether they were read from the database for an
/// export or from a file for an import. Every id a link names belongs to a record in the same snapshot.
/// </summary>
public sealed record DatasetSnapshot(
    string DatasetName,
    IReadOnlyList<SnapshotCategory> Categories,
    IReadOnlyList<SnapshotComponent> Components,
    IReadOnlyList<SnapshotBlueprint> Blueprints,
    IReadOnlyList<SnapshotFavorite> Favorites);
