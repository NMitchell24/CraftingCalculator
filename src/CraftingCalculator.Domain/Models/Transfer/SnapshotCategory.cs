namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>One category in a <see cref="DatasetSnapshot"/>.</summary>
public sealed record SnapshotCategory(int Id, string Name, string Description);
