using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.UI.Logging;
using CraftingCalculator.UI.Platform;
using Microsoft.Extensions.Logging;
using ForgeColors = CraftingCalculator.UI.Theme.AppTheme.ForgeColors;

namespace CraftingCalculator.UI;

/// <summary>
/// Shown in place of <see cref="MainPage" /> when the database could not be prepared at startup. Reports what
/// happened, where the log is, and on a platform with a downloads folder offers to save a copy of it.
/// </summary>
// Native controls rather than a BlazorWebView: the Blazor app is built on the database that just failed, so
// the screen that reports the failure must not depend on any of it. That is also why it carries no XAML - a
// page this small reads better as the tree it is.
internal sealed partial class StartupErrorPage : ContentPage
{
    private const string SaveLabel = "Save log to Downloads";

    private readonly IDiagnosticLog _log;
    private readonly IFileDownloader _downloader;
    private readonly ILogger<StartupErrorPage> _logger;

    private readonly Button _saveButton = new()
    {
        Text = SaveLabel,
        // A MAUI Button truncates by default, and this label is three words the user has to be able to read
        // at any system font size.
        LineBreakMode = LineBreakMode.WordWrap,
        HorizontalOptions = LayoutOptions.Fill,
        MinimumHeightRequest = 44,
        Padding = new Thickness(16, 12),
        // The dark palette's ink-on-copper pair is a button face in the Blazor app, where MudBlazor draws it
        // at weight 600; a MAUI Button has no such default and the label reads thin without this.
        FontAttributes = FontAttributes.Bold,
        BackgroundColor = Color.FromArgb(ForgeColors.DarkPrimary),
        TextColor = Color.FromArgb(ForgeColors.DarkOnAccent)
    };

    private readonly Label _status = new()
    {
        FontSize = 15,
        TextColor = Color.FromArgb(ForgeColors.DarkMuted),
        IsVisible = false
    };

    public StartupErrorPage(IDiagnosticLog log, IFileDownloader downloader, ILogger<StartupErrorPage> logger)
    {
        _log = log;
        _downloader = downloader;
        _logger = logger;

        _saveButton.IsVisible = downloader.IsSupported;
        _saveButton.Clicked += async (_, _) => await SaveLogAsync();

        // The splash slate, which is also .app-cloak's background (app.css) and the MauiSplashScreen color in
        // the csproj: this page replaces the screen the user was already looking at, so it keeps its ground
        // rather than flashing to a different one. Pinned, not themed - ThemeState lives in the Blazor scope
        // that never started, so there is no theme choice to follow. Every color below is the dark palette's,
        // which is the one designed to sit on this slate.
        BackgroundColor = Color.FromArgb(ForgeColors.LightSlate);

        // The window is edge-to-edge on Android 15+ (MainActivity) and iOS has its own insets; without this
        // the heading would sit under the status bar.
        SafeAreaEdges = SafeAreaEdges.All;

        // The system bars draw over this page, and the platform picks their icon set from the resolved app
        // theme - which on a light-themed device is the dark set, invisible on the slate above. Declaring the
        // page's own theme is what fixes that; it costs nothing app-wide, because when this page exists it is
        // the whole app.
        Microsoft.Maui.Controls.Application.Current!.UserAppTheme = AppTheme.Dark;

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(24),
                Spacing = 16,
                Children =
                {
                    Heading("Crafting Calculator couldn't start"),
                    Body("Your data hasn't been changed. The problem has been written to the log."),
                    Caption("Log file"),
                    Body(DiagnosticLogLocation.Describe(log)),
                    _saveButton,
                    _status
                }
            }
        };
    }

    private static Label Heading(string text) => new()
    {
        Text = text,
        FontSize = 24,
        FontAttributes = FontAttributes.Bold,
        TextColor = Color.FromArgb(ForgeColors.DarkInk)
    };

    private static Label Body(string text) => new()
    {
        Text = text,
        FontSize = 16,
        TextColor = Color.FromArgb(ForgeColors.DarkInk)
    };

    private static Label Caption(string text) => new()
    {
        Text = text,
        FontSize = 13,
        TextColor = Color.FromArgb(ForgeColors.DarkMuted)
    };

    private async Task SaveLogAsync()
    {
        // One save at a time: the cache copy DiagnosticLogDownload writes is named to the second, so two
        // overlapping runs would share a path that each truncates and then deletes.
        _saveButton.IsEnabled = false;
        _status.IsVisible = false;

        try
        {
            // ReadAll is synchronous file I/O; off the UI thread so the button's pressed state still paints.
            string text = await Task.Run(_log.ReadAll);
            await DiagnosticLogDownload.SaveAsync(_downloader, _logger, text);
            Report("Saved to your Downloads folder.");
        }
        catch (Exception exception)
        {
            LogSaveFailed(_logger, exception);
            Report("The log couldn't be saved. It's still on the device, where it says above.");
        }
        finally
        {
            _saveButton.IsEnabled = true;
        }
    }

    private void Report(string message)
    {
        _status.Text = message;
        _status.IsVisible = true;
    }

    [LoggerMessage(Level = LogLevel.Error,
        Message = "The log could not be saved to the Downloads folder from the startup error page")]
    private static partial void LogSaveFailed(ILogger logger, Exception exception);
}
