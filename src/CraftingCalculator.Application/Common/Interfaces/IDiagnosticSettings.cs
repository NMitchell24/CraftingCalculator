namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>The user's diagnostics preferences, persisted across launches.</summary>
public interface IDiagnosticSettings
{
    /// <summary>
    /// Whether the diagnostic log records every screen, dialog and command as well as errors. Off unless the user
    /// turns it on.
    /// </summary>
    bool TraceLogging { get; }

    /// <summary>Stores <paramref name="enabled"/> as <see cref="TraceLogging"/> and applies it to the log at once.</summary>
    void SetTraceLogging(bool enabled);
}
