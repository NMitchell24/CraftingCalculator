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
public sealed record PageAction(
    string Label, string Icon, Func<Task> OnClick, bool Disabled = false, bool Active = false);

/// <summary>
/// Everything a page puts on the shared shell. Only <see cref="Title"/> is required.
/// </summary>
public sealed record PageShellConfig(string Title)
{
    /// <summary>The page's actions, in the order the shell should show them.</summary>
    public IReadOnlyList<PageAction> Actions { get; init; } = [];

    /// <summary>Where the app bar's back arrow navigates, or null on a page that should not show one.</summary>
    public string? BackHref { get; init; }

    /// <summary>
    /// True when <see cref="Title"/> is user-entered data - a saved record's own name - rather than one
    /// of the app's own screen names. The layout renders its own titles in the display face and user
    /// content in the body face.
    /// </summary>
    public bool TitleIsUserContent { get; init; }
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
