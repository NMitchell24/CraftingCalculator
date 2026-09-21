using CraftingCalculator.Application.Common.Interfaces;

namespace CraftingCalculator.Application.Common.Services.Impl;

/// <summary>
/// The diagnostics preferences, stored through <see cref="IPreferenceStore"/> the same way
/// <see cref="ExportSettings"/> stores its own, and applied to <see cref="IDiagnosticLog"/> from the moment this is
/// constructed. Singleton: the log it drives is one per app.
/// </summary>
public sealed class DiagnosticSettings : IDiagnosticSettings
{
    private const string TraceLoggingKey = "trace_logging";

    private readonly IPreferenceStore _preferences;
    private readonly IDiagnosticLog _log;

    public DiagnosticSettings(IPreferenceStore preferences, IDiagnosticLog log)
    {
        _preferences = preferences;
        _log = log;

        // Anything unreadable is off, which is also the default.
        _log.TraceEnabled = bool.TryParse(preferences.Get(TraceLoggingKey), out bool stored) && stored;
    }

    public bool TraceLogging => _log.TraceEnabled;

    public void SetTraceLogging(bool enabled)
    {
        if (enabled == TraceLogging)
        {
            return;
        }

        // Stored first, so a failed write leaves the log as it was rather than on a level the next launch forgets.
        _preferences.Set(TraceLoggingKey, enabled.ToString());
        _log.TraceEnabled = enabled;
    }
}
