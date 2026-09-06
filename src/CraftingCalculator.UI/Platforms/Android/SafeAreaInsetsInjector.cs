using AndroidX.Core.View;
using Insets = AndroidX.Core.Graphics.Insets;

namespace CraftingCalculator.UI;

// On each inset pass: undo MAUI's system-bar padding so the WebView fills to the window edges, then
// push the current status/nav bar sizes into the page as CSS custom properties (--safe-area-inset-*)
// so the layout reserves that space and the app bar can draw behind a transparent status bar.
internal sealed class SafeAreaInsetsInjector(Android.Webkit.WebView webView)
    : Java.Lang.Object, IOnApplyWindowInsetsListener
{
    // Re-assert over the first few seconds. Both winning MAUI's inset listener (so the WebView draws
    // edge-to-edge) and landing the CSS-var injection on the loaded Blazor document are timing-sensitive,
    // and a single early pass loses one or the other on some devices (MAUI re-sets its listener; the
    // injection runs before the page finishes loading). Re-attaching and re-requesting insets a few
    // times, past page load, makes both reliable. Idempotent.
    public void AttachWithRetries(Android.Views.View content)
    {
        Attach(content);
        foreach (int delayMs in new[] { 300, 800, 1500, 3000 })
            content.PostDelayed(() => Attach(content), delayMs);
    }

    private void Attach(Android.Views.View content)
    {
        ViewCompat.SetOnApplyWindowInsetsListener(content, this);
        ViewCompat.RequestApplyInsets(content);
    }

    public WindowInsetsCompat? OnApplyWindowInsets(Android.Views.View? v, WindowInsetsCompat? insets)
    {
        if (v is null || insets is null)
            return insets;

        Insets? bars = insets.GetInsets(WindowInsetsCompat.Type.SystemBars());
        if (bars is null)
            return insets;

        v.SetPadding(0, 0, 0, 0);

        // Insets are device px; convert to CSS px in-page via the WebView's own devicePixelRatio
        // rather than guessing the display density (the two differ on some hardware).
        string js =
            $"document.documentElement.style.setProperty('--safe-area-inset-top',({bars.Top}/window.devicePixelRatio)+'px');" +
            $"document.documentElement.style.setProperty('--safe-area-inset-bottom',({bars.Bottom}/window.devicePixelRatio)+'px');";
        webView.EvaluateJavascript(js, null);

        // Consume only the system bars so no descendant re-applies them as padding; keep every other
        // inset type (notably IME, so keyboard resizing still works, and display cutout) flowing.
        // SetInsets is under-annotated as returning a nullable Builder; it always returns the builder.
        return new WindowInsetsCompat.Builder(insets)
            .SetInsets(WindowInsetsCompat.Type.SystemBars(), Insets.Of(0, 0, 0, 0))!
            .Build();
    }
}
