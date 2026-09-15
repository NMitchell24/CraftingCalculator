namespace CraftingCalculator.UI.State;

/// <summary>
/// One action a page contributes to the shell. Rendered as an icon button on the bottom actions bar in
/// portrait and as a nav item inside the drawer on wider viewports.
/// </summary>
/// <param name="Label">The action's accessible name, and its label wherever the shell shows text.</param>
/// <param name="Icon">A Material icon path, normally from <c>Icons.Material.Filled</c>.</param>
/// <param name="OnClick">Runs when the action is invoked.</param>
/// <param name="Disabled">True to render the action but refuse interaction.</param>
/// <param name="Active">True when the action is a mode the page is currently in, which the shell highlights.</param>
/// <param name="OnLongPress">
/// Runs instead of <paramref name="OnClick"/> when the action is pressed and held. Null on an action
/// that has no second gesture, which is what leaves an over-long press behaving as an ordinary tap.
/// </param>
public sealed record PageAction(
    string Label, string Icon, Func<Task> OnClick, bool Disabled = false, bool Active = false,
    Func<Task>? OnLongPress = null);

/// <summary>
/// Everything a page puts on the shared shell. Only <see cref="Title"/> is required.
/// </summary>
/// <param name="Title">
/// The screen's name in the app's own words, never user-entered data such as a record's name: the app bar
/// wraps it rather than truncating it, and renders it in the display face.
/// </param>
public sealed record PageShellConfig(string Title)
{
    /// <summary>The page's actions, in the order the shell should show them.</summary>
    public IReadOnlyList<PageAction> Actions { get; init; } = [];

    /// <summary>
    /// True to show the app bar's back arrow. It takes the same step back through history as the system back
    /// gesture, except on a help topic, where it opens the help contents.
    /// </summary>
    public bool ShowBack { get; init; }

    /// <summary>
    /// Asks whether the user may leave the page, resolving to true when they may; null on a page that can
    /// always be left. The shell awaits it before opening a destination from the bottom nav or side rail.
    /// </summary>
    public Func<Task<bool>>? ConfirmLeaveAsync { get; init; }
}

/// <summary>
/// Drives <c>MainLayout</c>'s shared chrome - the app bar's title and back arrow, and the page's
/// actions wherever the layout places them - so each page controls its own header without adding a
/// second app bar row. Scoped; pages call <see cref="Configure"/> in <c>OnInitialized</c> passing
/// themselves as the owner, and <see cref="Reset"/> with the same owner in <c>Dispose</c>.
/// </summary>
public sealed class PageShellState
{
    public const string DefaultTitle = "Crafting Calculator";

    private static readonly PageShellConfig Default = new(DefaultTitle);

    private object? _owner;

    public PageShellConfig Config { get; private set; } = Default;

    public event Action? Changed;

    /// <summary>Puts <paramref name="owner"/>'s configuration on the shared shell.</summary>
    public void Configure(object owner, PageShellConfig config)
    {
        _owner = owner;
        Config = config;
        Changed?.Invoke();
    }

    /// <summary>
    /// Restores the default title and drops the actions and back arrow, unless another page has taken
    /// the shell over in the meantime.
    /// </summary>
    public void Reset(object owner)
    {
        // Blazor renders the incoming page - running its Configure - before disposing the outgoing
        // one, so an unguarded reset here would wipe the title the new page just set.
        if (!ReferenceEquals(_owner, owner))
        {
            return;
        }

        _owner = null;
        Config = Default;
        Changed?.Invoke();
    }
}
