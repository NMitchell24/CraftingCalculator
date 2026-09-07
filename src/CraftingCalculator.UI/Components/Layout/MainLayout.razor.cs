using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private Breakpoint _breakpoint = Breakpoint.Md;

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
