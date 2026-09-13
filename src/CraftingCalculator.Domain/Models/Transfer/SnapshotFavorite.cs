namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// One favorite in a <see cref="DatasetSnapshot"/>. <see cref="Blueprints"/> link to
/// <see cref="SnapshotBlueprint"/>s in the same snapshot.
/// </summary>
public sealed record SnapshotFavorite(int Id, string Name, IReadOnlyList<QuantityLink> Blueprints);
