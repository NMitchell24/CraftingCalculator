using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.UI.Platform;

/// <summary>Where to tell the user the diagnostic log is, in the terms their platform gives them.</summary>
public static class DiagnosticLogLocation
{
    // Only a PC can show this as a path: the Android folder is invisible to every other app, and the iOS
    // sandbox path is not one the Files app displays.
#if WINDOWS
    /// <summary>The log's location, as a sentence or a path the user can act on.</summary>
    public static string Describe(IDiagnosticLog log) => log.FilePath;
#elif IOS
    /// <summary>The log's location, as a sentence or a path the user can act on.</summary>
    public static string Describe(IDiagnosticLog _) =>
        $"Files → On My {(DeviceInfo.Idiom == DeviceIdiom.Tablet ? "iPad" : "iPhone")} → Crafting Calculator → Logs";
#else
    /// <summary>The log's location, as a sentence or a path the user can act on.</summary>
    public static string Describe(IDiagnosticLog _) => "Saved in app storage";
#endif
}
