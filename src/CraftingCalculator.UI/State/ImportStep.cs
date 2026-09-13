namespace CraftingCalculator.UI.State;

/// <summary>The step the import wizard in <see cref="ImportState"/> has reached.</summary>
public enum ImportStep
{
    /// <summary>No file is chosen.</summary>
    SelectFile,

    /// <summary>The chosen file is being read and checked in the background.</summary>
    Validating,

    /// <summary>The chosen file can't be imported; <see cref="ImportState.ValidationErrors"/> says why.</summary>
    Invalid,

    /// <summary>The file's records are staged and the user is choosing which to import.</summary>
    Review,

    /// <summary>The chosen records are being compared with the dataset they are going into.</summary>
    CheckingConflicts,

    /// <summary>Some chosen records share a name with records in the target dataset, and the user hasn't said what to do.</summary>
    ConflictsFound,

    /// <summary>The user is picking, one conflict at a time, which of their records to replace.</summary>
    ResolveConflicts,

    /// <summary>The chosen records are being written.</summary>
    Importing
}
