namespace CraftingCalculator.UI.State;

public sealed record AppBarMenuItem(string Text, string Icon, Func<Task> OnClick);

/// <summary>
/// A page's single most important action, shown as an icon button on the app bar.
/// </summary>
public sealed record AppBarAction(string Label, string Icon, Func<Task> OnClick, bool Disabled = false);

/// <summary>
/// Everything a page puts on the shared app bar. Only <see cref="Title"/> is required.
/// </summary>
public sealed record AppBarConfig(string Title)
{
    public IReadOnlyList<AppBarMenuItem> MenuItems { get; init; } = [];

    /// <summary>Where the app bar's back arrow navigates, or null on a page that should not show one.</summary>
    public string? BackHref { get; init; }

    public AppBarAction? PrimaryAction { get; init; }

    /// <summary>
    /// True when <see cref="Title"/> is user-entered data - a saved record's own name - rather than one
    /// of the app's own screen names. The layout renders its own titles in the display face and user
    /// content in the body face.
    /// </summary>
    public bool TitleIsUserContent { get; init; }
}

/// <summary>
/// Drives <c>MainLayout</c>'s single shared app bar - title, primary action, and an optional overflow
/// menu - so each page controls its own header without adding a second app bar row. Scoped; pages call
/// <see cref="Configure"/> in <c>OnInitialized</c> passing themselves as the owner, and
/// <see cref="Reset"/> with the same owner in <c>Dispose</c>.
/// </summary>
public sealed class AppBarState
{
    public const string DefaultTitle = "Crafting Calculator";

    private static readonly AppBarConfig Default = new(DefaultTitle);

    private object? _owner;

    public AppBarConfig Config { get; private set; } = Default;

    public event Action? Changed;

    /// <summary>Puts <paramref name="owner"/>'s configuration on the shared app bar.</summary>
    public void Configure(object owner, AppBarConfig config)
    {
        _owner = owner;
        Config = config;
        Changed?.Invoke();
    }

    /// <summary>
    /// Restores the default title and drops the menu, primary action, and back arrow, unless another
    /// page has taken the bar over in the meantime.
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
