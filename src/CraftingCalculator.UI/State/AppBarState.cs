namespace CraftingCalculator.UI.State;

public sealed record AppBarMenuItem(string Text, string Icon, Func<Task> OnClick);

/// <summary>
/// Drives <c>MainLayout</c>'s single shared app bar - title and an optional overflow menu - so each
/// page controls its own header without adding a second app bar row. Scoped; pages call
/// <see cref="Configure"/> in <c>OnInitialized</c> passing themselves as the owner, and
/// <see cref="Reset"/> with the same owner in <c>Dispose</c>.
/// </summary>
public sealed class AppBarState
{
    public const string DefaultTitle = "Crafting Calculator";

    private object? _owner;

    public string Title { get; private set; } = DefaultTitle;
    public IReadOnlyList<AppBarMenuItem> MenuItems { get; private set; } = [];

    /// <summary>
    /// Where the app bar's back arrow navigates, or null on a page that should not show one.
    /// </summary>
    public string? BackHref { get; private set; }

    public event Action? Changed;

    /// <summary>Puts <paramref name="owner"/>'s title, menu, and back arrow on the shared app bar.</summary>
    public void Configure(object owner, string title, IReadOnlyList<AppBarMenuItem>? menuItems = null, string? backHref = null)
    {
        _owner = owner;
        Title = title;
        MenuItems = menuItems ?? [];
        BackHref = backHref;
        Changed?.Invoke();
    }

    /// <summary>
    /// Restores the default title and drops the menu and back arrow, unless another page has taken
    /// the bar over in the meantime.
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
        Title = DefaultTitle;
        MenuItems = [];
        BackHref = null;
        Changed?.Invoke();
    }
}
