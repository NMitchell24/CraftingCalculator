namespace CraftingCalculator.Domain.Constants;

/// <summary>
/// The range the user may set the export history to, shared by the Settings screen that offers it and the
/// setting that stores it.
/// </summary>
public static class ExportConstants
{
    /// <summary>How many exports the app keeps until the user says otherwise.</summary>
    public const int DefaultKeptExports = 5;

    /// <summary>The fewest exports the app can be set to keep, which is the newest one on its own.</summary>
    public const int MinimumKeptExports = 1;

    /// <summary>The most exports the app can be set to keep.</summary>
    public const int MaximumKeptExports = 20;
}
