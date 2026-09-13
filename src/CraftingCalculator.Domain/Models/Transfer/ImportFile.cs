namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// An export file that can be imported: the records it holds, when it was exported, and the version of the app that
/// exported it, as the file records it.
/// </summary>
public sealed record ImportFile(DatasetSnapshot Snapshot, DateTimeOffset ExportedAt, string AppVersion);
