using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// An incoming record that has the same name as a record already in the dataset it is imported into.
/// <see cref="IncomingId"/> is the record's id in the incoming snapshot, <see cref="ExistingId"/> the id of the
/// dataset's record, and <see cref="Name"/> the incoming record's name.
/// </summary>
public sealed record ImportConflict(RecordKind Kind, int IncomingId, int ExistingId, string Name);
