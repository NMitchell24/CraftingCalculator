using Microsoft.AspNetCore.Components.WebView;

namespace CraftingCalculator.UI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // The window is edge-to-edge on Android 15+ (MainActivity), so the WebView draws behind the system
    // bars. env(safe-area-inset-*) can't compensate there: the WebView maps it to the display cutout, not
    // the system bars, so it reads 0. SafeAreaInsetsInjector reads the real inset pixels and feeds them
    // into CSS custom properties the layout consumes instead (see wwwroot/app.css). No-op elsewhere - iOS
    // reports real safe-area insets to env() itself, and Windows has no system bars to clear.
    private void OnBlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
    {
#if ANDROID
        Android.Views.View? content =
            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.FindViewById(Android.Resource.Id.Content);
        if (content is not null)
        {
            new SafeAreaInsetsInjector(e.WebView).AttachWithRetries(content);
        }
#endif
    }
}
