using CraftingCalculator.UI.Components.Dialogs;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace CraftingCalculator.UI.State;

/// <summary>
/// Runs one of the app's commands - a save, a delete, a rename, an import - and takes the failure if there is
/// one: the exception goes to the log, the user is told in the caller's own words, and the screen they were on
/// is left exactly as it was, edits included. Scoped.
/// </summary>
/// <remarks>
/// A command is guarded; a page load is not. A throw on the way into a screen belongs to the page error
/// boundary, which replaces that screen with a card the user can leave from - there is nothing left to keep.
/// A command's screen is still perfectly usable, and an error card over it would throw away the work the user
/// was in the middle of.
/// </remarks>
public sealed partial class ActionGuard(IDialogService dialogs, ILogger<ActionGuard> logger)
{
    // One title for every failed command, so the app reports them in one voice. The caller's message is what
    // says which command it was.
    private const string FailureTitle = "That didn't work";

    // Said once here rather than in every caller's message. A retry is not advice - nothing these commands do
    // is a network call, so the usual cause is a bug, a full disk or a permission, and none of those clear on
    // a second tap. This is the one thing the user can actually do when trying it again does not help, and
    // Settings' Diagnostics card is where the log can be found. A caller whose own screen makes this line
    // nonsense - the log viewer, which is already showing the log - passes its own instead.
    private const string FailureHelp = "If it keeps happening, the log in Settings is the thing I need.";

    /// <summary>
    /// Runs <paramref name="action"/> and returns true when it finished. On failure the exception is logged,
    /// <paramref name="failureMessage"/> is put to the user, and this returns false.
    /// </summary>
    /// <param name="operation">
    /// What the app was doing, in the app's own words, as a compile-time constant such as
    /// <c>"DatasetEditor.Save"</c>. It is written to the log and read there by whoever is diagnosing the
    /// failure, so it never carries anything the user typed.
    /// </param>
    /// <param name="failureMessage">
    /// A sentence or two for the user naming what did not happen, and what state their work is in where that
    /// is not obvious from the screen. It never closes by telling them to try again: this appends the app's
    /// one line about what to do when a retry does not help.
    /// </param>
    /// <param name="action">The command to run.</param>
    /// <param name="help">
    /// What the user can do when running the command again does not help, closing the message. Defaults to
    /// pointing at the log in Settings, which is right everywhere except a screen already showing it.
    /// </param>
    public async Task<bool> RunAsync(
        string operation, string failureMessage, Func<Task> action, string help = FailureHelp)
    {
        LogActionStarted(logger, operation);

        try
        {
            await action();
            return true;
        }
        catch (Exception exception)
        {
            LogActionFailed(logger, exception, operation);

            // One paragraph: ConfirmDialog renders its message as a single MudText, so a line break here would
            // collapse. Three short sentences read fine as a run-on and it keeps that dialog's API alone.
            await ConfirmDialog.AlertAsync(dialogs, FailureTitle, $"{failureMessage} {help}");

            return false;
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Action started: {Operation}")]
    private static partial void LogActionStarted(ILogger logger, string operation);

    [LoggerMessage(Level = LogLevel.Error, Message = "Action failed: {Operation}")]
    private static partial void LogActionFailed(ILogger logger, Exception exception, string operation);
}
