using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// Identifies one record within a <see cref="DatasetSnapshot"/>. Ids are only unique per kind, so the kind
/// is part of the key.
/// </summary>
public readonly record struct RecordKey(RecordKind Kind, int Id);
