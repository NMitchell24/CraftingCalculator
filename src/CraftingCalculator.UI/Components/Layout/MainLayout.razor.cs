using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.UI.State;
using CraftingCalculator.UI.Theme;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;

namespace CraftingCalculator.UI.Components.Layout;

public partial class MainLayout : IBrowserViewportObserver, IDisposable
{
    [Inject] private PageShellState PageShellState { get; set; } = null!;
    [Inject] private ThemeState ThemeState { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private IBrowserViewportService ViewportService { get; set; } = null!;

    private const string SettingsRoute = "settings";

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private bool _themeResolved;
    private bool _cloakDismissed;
    // The viewport subscription is opened from OnAfterRenderAsync, so the first frame is painted
    // before NotifyBrowserViewportChangeAsync has ever run. Null is that unmeasured state for this
    // component's own layout decisions; MainLayout.razor currently cascades Breakpoint.Xs as a fallback
    // until the first viewport notification arrives.
    private Breakpoint? _breakpoint;
    private BrowserWindowSize? _windowSize;

    // Settings and Help are both app-bar overlays: they open over whatever page is showing, and their
    // own icon closes them back to it rather than to a fixed route. One return address each, so opening
    // Help from Settings and closing it again lands back on Settings instead of skipping past it.
    private string? _preSettingsUri;
    private string? _preHelpUri;

    // Below Sm, a fixed side rail costs too much horizontal space - the bottom nav takes over.
    // Neither renders while _breakpoint is null; the layout stays chrome-free for that one frame
    // rather than committing to a nav it may have to take back.
    private bool ShowBottomNav => _breakpoint == Breakpoint.Xs;
    private bool ShowSideRail => _breakpoint is not null && !ShowBottomNav;

    // The actions bar is the xs face of the page's actions; on every wider viewport the same actions
    // are folded into the drawer instead, so the two are never on screen together.
    private bool ShowActionsBar => ShowBottomNav && PageShellState.Config.Actions.Count > 0;

    // page-content-with-actions-bar restates the whole bottom offset rather than adding to it, so both
    // classes are applied together and its later declaration in app.css is what decides the padding.
    private string MainContentClass => (ShowBottomNav, ShowActionsBar) switch
    {
        (true, true) => "page-content page-content-with-bottom-nav page-content-with-actions-bar",
        (true, false) => "page-content page-content-with-bottom-nav",
        _ => "page-content"
    };

    // The labelled Persistent drawer is for genuine tablets and desktop windows only; every phone
    // gets the icon-only Mini rail whichever way it is turned. Breakpoint alone cannot make that
    // call - it is width-only, and a landscape phone (~890 CSS px) reads the same as a small tablet -
    // so this gates on height as well. Same 960x600 threshold, and the same reasoning, as the
    // two-column craft-columns media query in app.css.
    private bool ShowFullDrawer => _windowSize is { Width: >= 960, Height: >= 600 };

    // Retires the startup cloak (wwwroot/index.html). Both flags are set from OnAfterRenderAsync
    // callbacks, so the first frame this is true is also the first frame the layout is fully resolved.
    private bool IsReady => _themeResolved && _breakpoint is not null;

    // The display face is reserved for the app's own screen names; a record's own name renders in the
    // body face, which is both the honest signal and the legible choice for text the app never wrote.
    private string AppBarTitleClass =>
        PageShellState.Config.TitleIsUserContent ? "app-bar-title" : "app-bar-title display-title";

    protected override void OnInitialized()
    {
        PageShellState.Changed += StateHasChanged;
        ThemeState.Changed += OnThemeChanged;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyThemeAsync();
            await ViewportService.SubscribeAsync(this, fireImmediately: true);
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

    private string CurrentRoute => Navigation.ToBaseRelativePath(Navigation.Uri).TrimStart('/');

    private bool IsSettingsOpen => IsOpen(SettingsRoute);

    private bool IsHelpOpen => IsOpen(HelpTopics.HelpRoot);

    private string SettingsActionLabel => IsSettingsOpen ? "Close settings" : "Settings";

    private string HelpActionLabel => IsHelpOpen ? "Close help" : "Help for this screen";

    private void ToggleSettings() => ToggleOverlay(IsSettingsOpen, ref _preSettingsUri, $"/{SettingsRoute}");

    // The target is resolved from the route the user is on now, which is why it is computed here rather
    // than by the Help page itself: once the navigation has happened that route is gone.
    private void ToggleHelp() => ToggleOverlay(IsHelpOpen, ref _preHelpUri,
        $"/{HelpTopics.HelpRoot}/{HelpProcessor.ResolveTopic(CurrentRoute).Id}");

    /// <summary>Whether the current route is <paramref name="route"/> or a page beneath it.</summary>
    private bool IsOpen(string route) =>
        CurrentRoute.Equals(route, StringComparison.OrdinalIgnoreCase)
        || CurrentRoute.StartsWith($"{route}/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Opens <paramref name="target"/> over the current page, remembering where it was opened from in
    /// <paramref name="origin"/>, or closes it by navigating back there.
    /// </summary>
    private void ToggleOverlay(bool isOpen, ref string? origin, string target)
    {
        if (isOpen)
        {
            Navigation.NavigateTo(origin ?? "/");
            return;
        }

        origin = Navigation.Uri;
        Navigation.NavigateTo(target);
    }

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    Task IBrowserViewportObserver.NotifyBrowserViewportChangeAsync(BrowserViewportEventArgs args)
    {
        _breakpoint = args.Breakpoint;
        _windowSize = args.BrowserWindowSize;

        return InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        PageShellState.Changed -= StateHasChanged;
        ThemeState.Changed -= OnThemeChanged;

        // Fire and forget: IDisposable cannot await, and the subscription only holds a JS listener -
        // nothing downstream depends on the unsubscribe having completed.
        _ = ViewportService.UnsubscribeAsync(this);
    }
}
