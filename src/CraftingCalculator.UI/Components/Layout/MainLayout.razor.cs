using CraftingCalculator.UI.State;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private AppBarState AppBarState { get; set; } = null!;

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private Breakpoint _breakpoint = Breakpoint.Md;

    // Below Sm, a fixed side rail costs too much horizontal space - the bottom nav takes over.
    private bool ShowBottomNav => _breakpoint == Breakpoint.Xs;
    private bool ShowSideRail => !ShowBottomNav;

    protected override void OnInitialized()
    {
        AppBarState.Changed += StateHasChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _themeProvider.GetSystemDarkModeAsync();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnBreakpointChanged(Breakpoint breakpoint)
    {
        _breakpoint = breakpoint;
        StateHasChanged();
    }

    public void Dispose()
    {
        AppBarState.Changed -= StateHasChanged;
    }
}
