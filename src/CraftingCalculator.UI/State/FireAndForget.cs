namespace CraftingCalculator.UI.State;

/// <summary>
/// Runs work whose <see cref="Task"/> the caller discards, so nothing it throws is left to fault a task no
/// one observes. Without it such a failure only surfaces when the finalizer collects the task, which is
/// <c>TaskScheduler.UnobservedTaskException</c> - an entry in the log an arbitrary time later, and nothing on
/// screen.
/// </summary>
public static class FireAndForget
{
    /// <summary>
    /// Awaits <paramref name="work"/> and hands anything it throws to <paramref name="onFault"/>. The
    /// returned task is what the caller discards, and it never faults unless <paramref name="onFault"/>
    /// itself throws.
    /// </summary>
    /// <param name="work">The work to run.</param>
    /// <param name="onFault">
    /// Reports the failure. A component normally passes <c>DispatchExceptionAsync</c>, which routes it to the
    /// renderer the way an awaited <c>EventCallback</c> would; work started from a <c>Dispose</c> has no
    /// renderer left and logs instead.
    /// </param>
    public static async Task RunAsync(Func<Task> work, Func<Exception, Task> onFault)
    {
        try
        {
            await work();
        }
        catch (ObjectDisposedException)
        {
            // The component was torn down while the work was in flight, so there is no renderer left to
            // report to and nothing on screen the failure could still matter to.
        }
        catch (Exception exception)
        {
            await onFault(exception);
        }
    }
}
