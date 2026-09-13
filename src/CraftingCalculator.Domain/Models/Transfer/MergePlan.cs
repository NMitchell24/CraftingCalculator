namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// How an incoming snapshot is merged into a dataset. Every incoming record without a conflict is added as a new
/// record. A record with a conflict in <see cref="Conflicts"/> lands on the dataset's record instead: when its key
/// is in <see cref="Replace"/> its fields and links overwrite that record's, and otherwise the dataset's record is
/// kept as it is. Either way, whatever links to the incoming record links to the dataset's.
/// </summary>
public sealed record MergePlan(
    DatasetSnapshot Incoming,
    IReadOnlyList<ImportConflict> Conflicts,
    IReadOnlySet<RecordKey> Replace);
