namespace CraftingCalculator.Domain.Models;

/// <summary>
/// A batch of picked blueprints, and the blueprints nested in them that the batch takes as already crafted rather than
/// crafting.
/// </summary>
public sealed record Batch(IReadOnlyList<BlueprintQuantity> Blueprints, IReadOnlySet<int> AlreadyCraftedIds)
{
    /// <summary>A batch with no blueprints and nothing already crafted.</summary>
    public static Batch Empty { get; } = new([], new HashSet<int>());
}
