namespace CraftingCalculator.UI.State;

public sealed record AppBarMenuItem(string Text, string Icon, Func<Task> OnClick);

/// <summary>
/// Drives <c>MainLayout</c>'s single shared app bar - title and an optional overflow menu - so each
/// page controls its own header without adding a second app bar row. Scoped; pages call
/// <see cref="Configure"/> in <c>OnInitialized</c> and <see cref="Reset"/> in <c>Dispose</c>.
/// </summary>
public sealed class AppBarState
{
    public const string DefaultTitle = "Crafting Calculator";

    public string Title { get; private set; } = DefaultTitle;
    public IReadOnlyList<AppBarMenuItem> MenuItems { get; private set; } = [];

    public event Action? Changed;

    public void Configure(string title, IReadOnlyList<AppBarMenuItem>? menuItems = null)
    {
        Title = title;
        MenuItems = menuItems ?? [];
        Changed?.Invoke();
    }

    public void Reset() => Configure(DefaultTitle);
}
