using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.Components.Dialogs;
using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Settings : ComponentBase, IDisposable
{
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;
    [Inject] private IExportSettings ExportSettings { get; set; } = null!;
    [Inject] private IDiagnosticSettings DiagnosticSettings { get; set; } = null!;
    [Inject] private ActionGuard Guard { get; set; } = null!;
    [Inject] private ILogger<Settings> Logger { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    [CascadingParameter] private Breakpoint Breakpoint { get; set; }

    private int _traceSwitchGeneration;

    private static string Version => $"{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

    // No back arrow: Settings is an app-bar overlay, and the Settings icon that opened it is what closes it.
    protected override void OnInitialized() => PageShellState.Configure(this, new PageShellConfig("Settings"));

    private Task ShowLogsAsync() => LogViewerDialog.ShowAsync(DialogService, Breakpoint);

    private async Task SetTraceLoggingAsync(bool enabled)
    {
        bool saved = await Guard.RunAsync(
            "Settings.TraceLogging",
            "I couldn't change the trace logging setting.",
            () =>
            {
                DiagnosticSettings.SetTraceLogging(enabled);
                return Task.CompletedTask;
            });

        if (saved)
        {
            // Information, so the switch is on record whichever way it went: it is what explains a log that
            // suddenly fills with breadcrumbs, or stops having them.
            LogTraceLoggingChanged(Logger, enabled);
        }
        else
        {
            // A MudSwitch keeps the value it was toggled to, and re-rendering it with the same unchanged Value
            // does not put it back, so after a failed save it would show a setting that was never saved. A new
            // @key rebuilds it from the stored one.
            _traceSwitchGeneration++;
        }
    }

    public void Dispose() => PageShellState.Reset(this);

    [LoggerMessage(Level = LogLevel.Information, Message = "Trace logging enabled: {Enabled}")]
    private static partial void LogTraceLoggingChanged(ILogger logger, bool enabled);
}
