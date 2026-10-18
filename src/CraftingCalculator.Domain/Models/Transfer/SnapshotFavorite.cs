namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// One favorite in a <see cref="DatasetSnapshot"/>. <see cref="Blueprints"/> link to
/// <see cref="SnapshotBlueprint"/>s in the same snapshot, and <see cref="AlreadyCrafted"/> holds the ids of the ones
/// its batch takes as already crafted.
/// </summary>
public sealed record SnapshotFavorite(
    int Id, string Name, IReadOnlyList<QuantityLink> Blueprints, IReadOnlyList<int> AlreadyCrafted);
