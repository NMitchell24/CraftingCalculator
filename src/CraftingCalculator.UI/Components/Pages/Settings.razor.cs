using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;

namespace CraftingCalculator.UI.Components.Pages;

public partial class Settings : ComponentBase, IDisposable
{
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;

    private static string Version => $"{AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";

    // No BackHref: Settings is reached from the app bar rather than from one particular page, so the
    // always-present bottom nav / side rail is the way back rather than a fixed return route.
    protected override void OnInitialized() => AppBarState.Configure(this, new AppBarConfig("Settings"));

    public void Dispose() => AppBarState.Reset(this);
}
