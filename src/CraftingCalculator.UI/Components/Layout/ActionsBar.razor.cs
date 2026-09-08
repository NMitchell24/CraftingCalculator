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
/// nav drawer. Actions past <see cref="MaxVisible"/> collapse into an overflow menu, and an action that
/// supplies <see cref="PageAction.OnLongPress"/> runs that instead when it is pressed and held.
/// </summary>
public partial class ActionsBar : IDisposable
{
    [Parameter, EditorRequired] public IReadOnlyList<PageAction> Actions { get; set; } = [];

    [Parameter, EditorRequired] public ActionsBarMode Mode { get; set; }

    /// <summary>The most slots the bar will use, the overflow menu included.</summary>
    [Parameter] public int MaxVisible { get; set; } = 4;

    // At or under the cap everything fits; past it the last slot is spent on the overflow menu itself.
    // The floor is what keeps OverflowCount honest: MaxVisible is a public parameter, and a caller
    // passing 0 would otherwise make this -1 while Overflow still yields every action.
    private int VisibleCount => Actions.Count <= MaxVisible ? Actions.Count : Math.Max(MaxVisible - 1, 0);

    private IEnumerable<PageAction> Visible => Actions.Take(VisibleCount);

    private IEnumerable<PageAction> Overflow => Actions.Skip(VisibleCount);

    private int OverflowCount => Actions.Count - VisibleCount;

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

        // Task.Delay resumes off the renderer's sync context, so the handler has to be marshalled back.
        await InvokeAsync(onLongPress);
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
