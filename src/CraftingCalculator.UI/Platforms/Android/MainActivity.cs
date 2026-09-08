using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace CraftingCalculator.UI;

// SoftInput.AdjustResize rather than MAUI's default, Pan, which slides the whole window up when the
// keyboard opens and takes the status bar with it. App.xaml.cs sets the same mode through MAUI's own
// platform configuration - that is the one that wins at runtime; this keeps the manifest honest.
// Neither resizes the WebView under edge-to-edge, which is SafeAreaInsetsInjector's job.
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, WindowSoftInputMode = SoftInput.AdjustResize, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // On Android 15+ (SDK 35) declare edge-to-edge so the app bar can draw behind the transparent
        // status bar (MainPage feeds the system-bar insets into CSS variables the layout consumes).
        // 35 == BuildVersionCodes.VanillaIceCream (Android 15). Literal, not the enum member, which is
        // [SupportedOSPlatform("android35.0")] and would trip CA1416 when named on this API-24-reachable site.
        if (Window is not null && OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
        }
    }
}
