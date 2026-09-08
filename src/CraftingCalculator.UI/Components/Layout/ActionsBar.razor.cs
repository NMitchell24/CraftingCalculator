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
/// nav drawer. Actions past <see cref="MaxVisible"/> collapse into an overflow menu.
/// </summary>
public partial class ActionsBar
{
    [Parameter, EditorRequired] public IReadOnlyList<PageAction> Actions { get; set; } = [];

    [Parameter, EditorRequired] public ActionsBarMode Mode { get; set; }

    /// <summary>The most slots the bar will use, the overflow menu included.</summary>
    [Parameter] public int MaxVisible { get; set; } = 4;

    // At or under the cap everything fits; past it the last slot is spent on the overflow menu itself.
    private int VisibleCount => Actions.Count <= MaxVisible ? Actions.Count : MaxVisible - 1;

    private IEnumerable<PageAction> Visible => Actions.Take(VisibleCount);

    private IEnumerable<PageAction> Overflow => Actions.Skip(VisibleCount);

    private int OverflowCount => Actions.Count - VisibleCount;

    private static Color ColorFor(PageAction action) => action.Active ? Color.Primary : Color.Default;
}
