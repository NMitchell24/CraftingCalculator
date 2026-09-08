using CraftingCalculator.UI.State;
using CraftingCalculator.UI.Theme;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace CraftingCalculator.UI.Components.Layout;

public partial class MainLayout : IDisposable
{
    [Inject] private AppBarState AppBarState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;

    private const string SettingsRoute = "settings";

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private bool _themeResolved;
    private bool _cloakDismissed;
    // MudBreakpointProvider subscribes to IBrowserViewportService from its own OnAfterRenderAsync, so
    // the first frame is painted before OnBreakpointChanged has ever run. Null is that unmeasured
    // state - seeding a concrete breakpoint instead makes the app open on whichever nav the seed
    // happened to pick and swap once the real viewport arrives.
    private Breakpoint? _breakpoint;

    // Where Settings was opened from, so the gear closes back to it rather than to a fixed route.
    private string? _preSettingsUri;

    // Below Sm, a fixed side rail costs too much horizontal space - the bottom nav takes over.
    // Neither renders while _breakpoint is null; the layout stays chrome-free for that one frame
    // rather than committing to a nav it may have to take back.
    private bool ShowBottomNav => _breakpoint == Breakpoint.Xs;
    private bool ShowSideRail => _breakpoint is not null && !ShowBottomNav;

    // Retires the startup cloak (wwwroot/index.html). Both flags are set from OnAfterRenderAsync
    // callbacks, so the first frame this is true is also the first frame the layout is fully resolved.
    private bool IsReady => _themeResolved && _breakpoint is not null;

    // The display face is reserved for the app's own screen names; a record's own name renders in the
    // body face, which is both the honest signal and the legible choice for text the app never wrote.
    private string AppBarTitleClass =>
        AppBarState.Config.TitleIsUserContent ? "app-bar-title" : "app-bar-title display-title";

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

        // Here rather than anywhere the state changes, so the frame the cloak fades off is the resolved
        // one. The cloak is plain markup outside #app - it has been on screen since before Blazor
        // started, so this is a dismissal, not a render.
        if (IsReady && !_cloakDismissed)
        {
            _cloakDismissed = true;
            await Js.InvokeVoidAsync("appCloak.dismiss");
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

        _themeResolved = true;
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
