using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.UI.Components.Controls;

/// <summary>
/// An <see cref="ErrorBoundary" /> that logs every exception it captures under the name of the region it
/// guards, and tells its owner about the failure through <see cref="OnFaulted" />. Recovering it is the
/// owner's job: <c>MainLayout</c> recovers the page boundary when the router hands it a new page.
/// </summary>
public partial class LoggingErrorBoundary : ErrorBoundary
{
    // One notification per failure, rather than one per render the ErrorContent goes through.
    private bool _faultReported;

    [Inject] private ILogger<LoggingErrorBoundary> Logger { get; set; } = null!;

    /// <summary>
    /// Which region this boundary guards - "page", "dialog" or "app". It is the app's own name for that
    /// region, and it is written to the log with every exception this boundary catches.
    /// </summary>
    [Parameter, EditorRequired] public string Scope { get; set; } = "";

    /// <summary>
    /// Raised once per failure, after the exception has been logged and <c>ErrorContent</c> has rendered in
    /// place of the faulted subtree. For an owner that has to clean up after what the boundary tore down.
    /// </summary>
    [Parameter] public EventCallback OnFaulted { get; set; }

    protected override Task OnErrorAsync(Exception exception)
    {
        // Replaces the base implementation rather than extending it: the base writes its own entry under a
        // Microsoft.* category, which this app's file log filters to Warning, so logging here keeps each
        // failure to a single entry under the app's own category.
        LogBoundaryCaught(Logger, exception, Scope);
        return Task.CompletedTask;
    }

    // OnFaulted is raised from here rather than from OnErrorAsync because of what the dialog boundary's
    // handler has to do. The faulted MudDialogProvider is only disposed by the render that swaps ErrorContent
    // in, and ErrorBoundaryBase sets CurrentException and re-renders after OnErrorAsync has returned - an
    // owner told from there would recover the boundary before the teardown it is recovering from.
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (CurrentException is null)
        {
            _faultReported = false;
            return;
        }

        if (_faultReported)
        {
            return;
        }

        _faultReported = true;
        await OnFaulted.InvokeAsync();
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Unhandled exception reached the {Scope} error boundary")]
    private static partial void LogBoundaryCaught(ILogger logger, Exception exception, string scope);
}
