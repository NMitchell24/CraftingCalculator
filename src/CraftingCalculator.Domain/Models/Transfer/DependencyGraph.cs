namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// Which records of a <see cref="DatasetSnapshot"/> need which others. Every key in <see cref="All"/> has an
/// entry, possibly empty, in both <see cref="DependsOn"/> and <see cref="UsedBy"/>.
/// </summary>
/// <param name="All">Every record in the snapshot.</param>
/// <param name="DependsOn">The records each record links to directly.</param>
/// <param name="UsedBy">The records that link directly to each record: <paramref name="DependsOn"/> reversed.</param>
public sealed record DependencyGraph(
    IReadOnlyList<RecordKey> All,
    IReadOnlyDictionary<RecordKey, IReadOnlyList<RecordKey>> DependsOn,
    IReadOnlyDictionary<RecordKey, IReadOnlyList<RecordKey>> UsedBy);
