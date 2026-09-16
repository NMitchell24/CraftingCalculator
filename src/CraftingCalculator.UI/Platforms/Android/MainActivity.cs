using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Activity;
using AndroidX.Core.View;
using CraftingCalculator.UI.State;

namespace CraftingCalculator.UI;

// SoftInput.AdjustResize rather than MAUI's default, Pan, which slides the whole window up when the
// keyboard opens and takes the status bar with it. App.xaml.cs sets the same mode through MAUI's own
// platform configuration - that is the one that wins at runtime; this keeps the manifest honest.
// Neither resizes the WebView under edge-to-edge, which is SafeAreaInsetsInjector's job.
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, WindowSoftInputMode = SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private BackButtonState _backButtonState = null!;
    private BackButtonCallback _backCallback = null!;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // On Android 15+ (SDK 35) declare edge-to-edge so the app bar can draw behind the transparent
        // status bar (MainPage feeds the system-bar insets into CSS variables the layout consumes).
        // 35 == BuildVersionCodes.VanillaIceCream (Android 15). Literal, not the enum member, which is
        // [SupportedOSPlatform("android35.0")] and would trip CA1416 when named on this API-29-reachable site.
        if (Window is not null && OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
        }

        _backButtonState = IPlatformApplication.Current!.Services.GetRequiredService<BackButtonState>();
        _backCallback = new BackButtonCallback(_backButtonState);
        _backButtonState.Changed += OnBackButtonChanged;
        OnBackButtonChanged();
    }

    protected override void OnDestroy()
    {
        _backButtonState.Changed -= OnBackButtonChanged;
        _backCallback.Remove();

        base.OnDestroy();
    }

    private void OnBackButtonChanged()
    {
        // The dispatcher asks the most recently added enabled callback first. BlazorWebView adds its own in
        // ConnectHandler, after OnCreate, and enables it whenever the WebView can go back, so a callback added once
        // here would lose to it on every page but the root. Re-adding this one each time a handler is set puts it in
        // front; removing it the rest of the time leaves back entirely to MAUI.
        _backCallback.Remove();

        if (_backButtonState.Handler is not null)
        {
            OnBackPressedDispatcher.AddCallback(this, _backCallback);
        }
    }

    internal sealed class BackButtonCallback(BackButtonState state) : OnBackPressedCallback(true)
    {
        public override void HandleOnBackPressed() => state.Handler?.Invoke();
    }
}
