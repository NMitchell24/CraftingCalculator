using CraftingCalculator.Domain.Enums;

namespace CraftingCalculator.Domain.Models.Transfer;

/// <summary>
/// What a selection action would do, worked out before it is applied: the keys it would add to the
/// selection and remove from it.
/// </summary>
/// <param name="Added">Keys not currently selected that the action selects.</param>
/// <param name="Removed">Keys currently selected that the action deselects.</param>
/// <param name="CascadedByKind">
/// How many of <paramref name="Added"/> or <paramref name="Removed"/> are records other than the ones the
/// user acted on, per kind. A kind with none is absent.
/// </param>
public sealed record SelectionChange(
    IReadOnlySet<RecordKey> Added,
    IReadOnlySet<RecordKey> Removed,
    IReadOnlyDictionary<RecordKind, int> CascadedByKind);
