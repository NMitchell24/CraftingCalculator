namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// A quantity of another record in the same <see cref="DatasetSnapshot"/>, named by its id. The kind of
/// record is implied by the list the link sits in.
/// </summary>
public sealed record QuantityLink(int TargetId, long Quantity);
