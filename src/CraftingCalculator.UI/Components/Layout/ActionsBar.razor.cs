using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

// MudBlazor.Color and Microsoft.Maui.Graphics.Color are both in scope in this project's global usings.
using Color = MudBlazor.Color;

namespace CraftingCalculator.UI.Components.Layout;

/// <summary>Where <see cref="ActionsBar"/> is being rendered, which decides the shape it takes.</summary>
public enum ActionsBarMode
{
    /// <summary>A row of icon buttons fixed above the bottom navigation.</summary>
    BottomBar,

    /// <summary>Nav items folded into the side drawer's menu, below the app's destinations.</summary>
    Drawer
}

/// <summary>
/// Renders a page's <see cref="PageAction"/>s, either as the bottom actions bar or as extra rows in the
/// nav drawer. Actions past <see cref="MaxVisible"/> collapse into an overflow menu, taken from the end of
/// the list but never an action that is <see cref="PageAction.Active"/> or supplies
/// <see cref="PageAction.OnLongPress"/> while a plain action could go instead. An action that supplies
/// <see cref="PageAction.OnLongPress"/> runs that instead of its click when it is pressed and held.
/// </summary>
public partial class ActionsBar : IDisposable
{
    [Parameter, EditorRequired] public IReadOnlyList<PageAction> Actions { get; set; } = [];

    [Parameter, EditorRequired] public ActionsBarMode Mode { get; set; }

    /// <summary>
    /// The most slots the bar will use, the overflow menu included. <see cref="int.MaxValue"/> means the
    /// actions never overflow.
    /// </summary>
    [Parameter, EditorRequired] public int MaxVisible { get; set; }

    // At or under the cap everything fits; past it the last slot is spent on the overflow menu itself.
    private int VisibleCount => Actions.Count <= MaxVisible ? Actions.Count : MaxVisible - 1;

    private List<PageAction> _visible = [];
    private List<PageAction> _overflow = [];

    protected override void OnParametersSet()
    {
        // A menu item can be neither highlighted nor held, so an action that is Active or has a long press
        // claims a visible slot before any plain action does. OrderBy is stable, so both lists keep page
        // order.
        HashSet<int> visibleIndexes =
        [
            .. Enumerable.Range(0, Actions.Count)
                .OrderBy(index => Actions[index].Active || Actions[index].OnLongPress is not null ? 0 : 1)
                .Take(VisibleCount)
        ];

        _visible = [.. Actions.Where((_, index) => visibleIndexes.Contains(index))];
        _overflow = [.. Actions.Where((_, index) => !visibleIndexes.Contains(index))];
    }

    private static Color ColorFor(PageAction action) => action.Active ? Color.Primary : Color.Default;

    // Android's own threshold (ViewConfiguration.getLongPressTimeout), so a hold that feels long here
    // feels long everywhere else on the device.
    private const int LongPressMilliseconds = 500;

    private CancellationTokenSource? _longPress;
    private bool _longPressFired;

    private void BeginLongPress(PageAction action)
    {
        StopLongPress();
        _longPressFired = false;

        if (action.OnLongPress is null || action.Disabled)
        {
            return;
        }

        CancellationTokenSource pressed = new();
        _longPress = pressed;

        _ = RunLongPressAsync(action.OnLongPress, pressed.Token);
    }

    private async Task RunLongPressAsync(Func<Task> onLongPress, CancellationToken token)
    {
        try
        {
            await Task.Delay(LongPressMilliseconds, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        // Set before the handler runs, not after: releasing the finger fires pointerup and then click
        // in immediate succession, and InvokeActionAsync reads this to swallow that click.
        _longPressFired = true;

        try
        {
            // Task.Delay resumes off the renderer's sync context, so the handler has to be marshalled back.
            await InvokeAsync(onLongPress);
        }
        catch (ObjectDisposedException)
        {
            // The component was torn down while the handler ran, so there is no renderer left to
            // report to. Dispose cancels the token, but only the Task.Delay above observes it.
        }
        catch (Exception exception)
        {
            // BeginLongPress discards this task, so an escaping exception would fault it unobserved
            // rather than surfacing. DispatchExceptionAsync routes it to the renderer the way an
            // awaited EventCallback would.
            await DispatchExceptionAsync(exception);
        }
    }

    private void StopLongPress()
    {
        _longPress?.Cancel();
        _longPress?.Dispose();
        _longPress = null;
    }

    private Task InvokeActionAsync(PageAction action)
    {
        if (!_longPressFired)
        {
            return action.OnClick();
        }

        _longPressFired = false;
        return Task.CompletedTask;
    }

    public void Dispose() => StopLongPress();
}
