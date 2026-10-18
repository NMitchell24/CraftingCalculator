namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A batch of picked blueprints, and the blueprints nested in them that the batch takes as already crafted rather than
/// crafting.
/// </summary>
public sealed record Batch(IReadOnlyList<BlueprintQuantity> Blueprints, IReadOnlySet<int> AlreadyCraftedIds);
