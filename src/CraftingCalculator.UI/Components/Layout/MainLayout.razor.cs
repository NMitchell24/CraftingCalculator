using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private const string SettingsRoute = "settings";

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private Breakpoint _breakpoint = Breakpoint.Md;

    // Where Settings was opened from, so the gear closes back to it rather than to a fixed route.
    private string? _preSettingsUri;

    // Below Sm, a fixed side rail costs too much horizontal space - the bottom nav takes over.
    private bool ShowBottomNav => _breakpoint == Breakpoint.Xs;
    private bool ShowSideRail => !ShowBottomNav;

    protected override void OnInitialized()
    {
        AppBarState.Changed += StateHasChanged;
        ThemeState.Changed += OnThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyThemeAsync();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    // Raised by the Settings page, so this has to reach the renderer's dispatcher rather than run as
    // an async void handler - ApplyThemeAsync does JS interop for the System case.
    private void OnThemeChanged() => _ = InvokeAsync(async () =>
    {
        await ApplyThemeAsync();
        StateHasChanged();
    });

    private async Task ApplyThemeAsync()
    {
        _isDarkMode = ThemeState.Mode switch
        {
            ThemeMode.Light => false,
            ThemeMode.Dark => true,
            _ => await _themeProvider.GetSystemDarkModeAsync()
        };
    }

    private bool IsSettingsOpen =>
        Navigation.ToBaseRelativePath(Navigation.Uri).TrimStart('/')
            .StartsWith(SettingsRoute, StringComparison.OrdinalIgnoreCase);

    private string SettingsActionLabel => IsSettingsOpen ? "Close settings" : "Settings";

    private void ToggleSettings()
    {
        if (IsSettingsOpen)
        {
            Navigation.NavigateTo(_preSettingsUri ?? "/");
            return;
        }

        _preSettingsUri = Navigation.Uri;
        Navigation.NavigateTo($"/{SettingsRoute}");
    }

    private void OnBreakpointChanged(Breakpoint breakpoint)
    {
        _breakpoint = breakpoint;
        StateHasChanged();
    }

    public void Dispose()
    {
        AppBarState.Changed -= StateHasChanged;
        ThemeState.Changed -= OnThemeChanged;
    }
}
