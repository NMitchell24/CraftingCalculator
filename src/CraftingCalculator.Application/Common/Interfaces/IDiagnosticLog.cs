namespace CraftingCalculator.Application.Common.Interfaces;

/// <summary>The diagnostic log the app writes on the device, for the UI to show and hand to the user.</summary>
public interface IDiagnosticLog
{
    /// <summary>Full path of the current log file.</summary>
    string FilePath { get; }

    /// <summary>
    /// All retained log text, oldest entry first. Empty when nothing has been logged or the log cannot be read.
    /// </summary>
    string ReadAll();
}
