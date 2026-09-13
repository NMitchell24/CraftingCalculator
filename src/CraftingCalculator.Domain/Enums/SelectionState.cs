namespace CraftingCalculator.Domain.Enums;

/// <summary>How much of one <see cref="RecordKind"/> an import or export selection holds.</summary>
public enum SelectionState
{
    /// <summary>No record of the kind is selected, including when the kind has no records at all.</summary>
    None,

    /// <summary>At least one record of the kind is selected, and at least one is not.</summary>
    Some,

    /// <summary>Every record of the kind is selected.</summary>
    All
}
