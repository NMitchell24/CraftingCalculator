using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Application.Common.Interfaces;
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
    [Inject] private IPreferenceStore PreferenceStore { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private IJSRuntime Js { get; set; } = null!;
    [Inject] private IBrowserViewportService ViewportService { get; set; } = null!;

    private const string SettingsRoute = "settings";

    private const string DrawerCollapsedKey = "drawer_collapsed";

    // The bottom actions bar's slot count, and a phone's in the drawer too: a landscape phone's rail is
    // too short to hold more than that below the three destinations.
    private const int PhoneActionsMaxVisible = 4;

    // The drawer's geometry in CSS px, for fitting a tablet's actions to its height. Measured with
    // getBoundingClientRect in the Android WebView (Pixel 9 emulator, landscape Mini rail, MudBlazor
    // 9.7.0): the Dense app bar without its status-bar padding, one MudNavLink row, and
    // .actions-bar-divider's 1px rule plus its 8px margins. Re-measure after a MudBlazor upgrade.
    private const int DestinationCount = 3;
    private const double AppBarHeight = 48;
    private const double NavLinkHeight = 40;
    private const double ActionsDividerHeight = 17;

    private MudThemeProvider _themeProvider = null!;
    private bool _isDarkMode;
    private bool _themeResolved;
    private bool _cloakDismissed;
    private bool _drawerCollapsed;
    // The viewport subscription is opened from OnAfterRenderAsync, so the first frame is painted
    // before NotifyBrowserViewportChangeAsync has ever run. Null is that unmeasured state for this
    // component's own layout decisions; MainLayout.razor currently cascades Breakpoint.Xs as a fallback
    // until the first viewport notification arrives.
    private Breakpoint? _breakpoint;
    private BrowserWindowSize? _windowSize;

    // Settings and Help are both app-bar overlays: they open over whatever page is showing, and their
    // own icon closes them back to it rather than to a fixed route. One shared stack rather than a
    // return address each, because the overlays open over each other: opening Help from Settings has to
    // close back to Settings, and closing that has to reach the page Settings was opened from. Two
    // slots would point at each other there, and the pair would never unwind.
    private readonly Stack<string> _overlayOrigins = new();

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

    // The user's collapse choice only applies where the labelled drawer could show; below the threshold
    // the Mini rail wins regardless, and growing the window back restores the choice.
    private bool DrawerExpanded => ShowFullDrawer && !_drawerCollapsed;

    private string DrawerToggleIcon => _drawerCollapsed ? Icons.Material.Filled.Menu : Icons.Material.Filled.MenuOpen;

    private string DrawerToggleLabel => _drawerCollapsed ? "Expand menu" : "Collapse menu";

    private int ActionsMaxVisible
    {
        get
        {
            DeviceIdiom idiom = DeviceInfo.Current.Idiom;

            if (ShowBottomNav || idiom == DeviceIdiom.Phone)
            {
                return PhoneActionsMaxVisible;
            }

            // A desktop window has a scroll wheel and is resized at will, so it shows every action and the
            // drawer scrolls (app.css) rather than hiding actions behind a menu.
            if (idiom == DeviceIdiom.Desktop)
            {
                return int.MaxValue;
            }

            // Every other idiom is a tablet in practice. The status-bar inset above the app bar is not known
            // here (Android injects it into CSS only), so this can overcount by one slot on a tall inset;
            // the drawer's own scroll absorbs that.
            double free = (_windowSize?.Height ?? 0) - AppBarHeight - DestinationCount * NavLinkHeight
                          - ActionsDividerHeight;

            return Math.Max(PhoneActionsMaxVisible, (int)Math.Floor(free / NavLinkHeight));
        }
    }

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

        // An absent or unrecognized stored value leaves the drawer expanded.
        bool.TryParse(PreferenceStore.Get(DrawerCollapsedKey), out _drawerCollapsed);
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

    private void ToggleDrawer()
    {
        _drawerCollapsed = !_drawerCollapsed;
        PreferenceStore.Set(DrawerCollapsedKey, _drawerCollapsed.ToString());
    }

    private string CurrentRoute => Navigation.ToBaseRelativePath(Navigation.Uri).TrimStart('/');

    private bool IsSettingsOpen => IsOpen(SettingsRoute);

    private bool IsHelpOpen => IsOpen(HelpTopics.HelpRoot);

    private string SettingsActionLabel => IsSettingsOpen ? "Close settings" : "Settings";

    private string HelpActionLabel => IsHelpOpen ? "Close help" : "Help for this screen";

    private void ToggleSettings() => ToggleOverlay(IsSettingsOpen, $"/{SettingsRoute}");

    // The target is resolved from the route the user is on now, which is why it is computed here rather
    // than by the Help page itself: once the navigation has happened that route is gone.
    private void ToggleHelp() => ToggleOverlay(IsHelpOpen,
        $"/{HelpTopics.HelpRoot}/{HelpProcessor.ResolveTopic(CurrentRoute).Id}");

    /// <summary>Whether the current route is <paramref name="route"/> or a page beneath it.</summary>
    private bool IsOpen(string route) =>
        CurrentRoute.Equals(route, StringComparison.OrdinalIgnoreCase)
        || CurrentRoute.StartsWith($"{route}/", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Opens <paramref name="target"/> over the current page, or closes it by navigating back to
    /// wherever the topmost open overlay was opened from.
    /// </summary>
    private void ToggleOverlay(bool isOpen, string target)
    {
        if (isOpen)
        {
            // Empty on a deep link straight into an overlay, or once the user has left one by the nav
            // rail rather than by its own icon.
            Navigation.NavigateTo(_overlayOrigins.Count > 0 ? _overlayOrigins.Pop() : "/");
            return;
        }

        // Opening an overlay from an ordinary page starts a new chain. Anything still on the stack was
        // left by an overlay the user walked away from with the nav rail instead of closing, and popping
        // it later would send them back to a page they had already moved on from.
        if (!IsSettingsOpen && !IsHelpOpen)
        {
            _overlayOrigins.Clear();
        }

        _overlayOrigins.Push(Navigation.Uri);
        Navigation.NavigateTo(target);
    }

    Guid IBrowserViewportObserver.Id { get; } = Guid.NewGuid();

    // MudBlazor notifies on breakpoint changes only by default, and breakpoints are width-only, so a
    // height-only resize (a desktop window dragged shorter) never reached ShowFullDrawer's height gate or
    // ActionsMaxVisible's fit-to-height.
    ResizeOptions IBrowserViewportObserver.ResizeOptions { get; } = new() { NotifyOnBreakpointOnly = false };

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
