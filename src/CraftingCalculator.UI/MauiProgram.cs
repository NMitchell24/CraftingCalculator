using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Infrastructure;
using CraftingCalculator.UI.Logging;
using CraftingCalculator.UI.Platform;
using CraftingCalculator.UI.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
#if IOS
using Foundation;
#endif

namespace CraftingCalculator.UI;

public static partial class MauiProgram
{
    // Not "CraftingCalculator.Startup": the category is a prefix the logging filters match on, and this one
    // has to stay clear of the app's own namespaces so a future filter cannot silence the session header.
    private const string StartupCategory = "Startup";

    /// <summary>
    /// Whether the database was prepared successfully; false means the app has no data layer and
    /// <see cref="App.CreateWindow" /> shows <see cref="StartupErrorPage" /> instead of <see cref="MainPage" />.
    /// </summary>
    internal static bool DatabaseReady { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices(config =>
        {
            // MudBlazor's default is TopRight, which puts every toast squarely on the app bar's
            // primary-action / settings / overflow cluster at z-index 1500 - so a "Deleted 'x'" toast
            // blocks the + that creates the next one. Docked at the bottom instead, offset above the
            // totals bar and bottom nav in app.css.
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = true;
            config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;

            // app.css makes snackbars pointer-events:none so a tap always reaches the control beneath,
            // which would leave a close button that does nothing.
            config.SnackbarConfiguration.ShowCloseIcon = false;

            // Defaults total 8s on screen (1000 + 5000 + 2000), most of it a slow fade still covering
            // whatever is underneath.
            config.SnackbarConfiguration.ShowTransitionDuration = 150;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 300;
        });

        Application.DependencyInjection.AddApplicationServices(builder.Services);
        builder.Services.AddDatabaseServices(GetDatabasePath());
        builder.Services.AddExportFileServices(GetExportsPath());

        builder.Services.AddSingleton<BackButtonState>();
        builder.Services.AddSingleton<IClipboardService, ClipboardService>();
        builder.Services.AddSingleton<IFileDownloader, FileDownloader>();
        builder.Services.AddSingleton<IImportFilePicker, ImportFilePicker>();
        builder.Services.AddSingleton<IPreferenceStore, PreferenceStore>();
        builder.Services.AddSingleton<IShareService, ShareService>();

        builder.Services.AddScoped<ActionGuard>();
        builder.Services.AddScoped<CategoryFilterState>();
        builder.Services.AddScoped<CraftState>();
        builder.Services.AddScoped<ExportState>();
        builder.Services.AddScoped<ImportState>();
        builder.Services.AddScoped<PageShellState>();
        builder.Services.AddScoped<ThemeState>();

        // The one page in the container, because App.CreateWindow has to build it from services rather than
        // with new. Transient: on the happy path it is never constructed at all.
        builder.Services.AddTransient<StartupErrorPage>();

        // The file log ships in Release: it is the only diagnostics a device in the field has.
#if DEBUG
        // Unredacted on purpose, so a development log stays readable; the session header says so, and the
        // choice is made here rather than inside the sink so its tests run in any configuration.
        builder.Logging.AddFileLogging(GetLogsPath());
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#else
        builder.Logging.AddRedactedFileLogging(GetLogsPath(), RedactedRoots());
#endif

        MauiApp app = builder.Build();

        ILogger startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(StartupCategory);

        // Before the header and the database work below: everything from here on is already covered by the
        // last-resort hooks, and a failure in startup itself is exactly what has nowhere else to be recorded.
        GlobalExceptionHandler.Install(startupLogger);

        LogSessionStarted(startupLogger, SessionHeader());

        try
        {
            // The DB must exist (and be migrated) before any page loads, and a dataset must be selected
            // before anything reads a record - every query is scoped to one, and DatasetScopedContextFactory
            // throws rather than hand out a context with no dataset.
            using IServiceScope scope = app.Services.CreateScope();
            IDbContextFactory<CraftingDataContext> contextFactory =
                scope.ServiceProvider.GetRequiredService<IDbContextFactory<CraftingDataContext>>();
            using CraftingDataContext context = contextFactory.CreateDbContext();
            context.Database.Migrate();

            // Blocking, matching Migrate() above: CreateMauiApp is synchronous, and the app must not
            // reach its first page until the selection is resolved.
            scope.ServiceProvider.GetRequiredService<IDatasetService>().InitializeAsync().GetAwaiter().GetResult();

            DatabaseReady = true;
        }
        catch (Exception exception)
        {
            // Swallowed rather than rethrown: this runs before any UI exists, so a throw here is a launch that
            // dies with nothing on screen. StartupErrorPage reports it instead, and the entry above is what
            // makes the failure diagnosable.
            LogStartupFailed(startupLogger, exception);
        }

        return app;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Session started\n{Header}")]
    private static partial void LogSessionStarted(ILogger logger, string header);

    [LoggerMessage(Level = LogLevel.Critical,
        Message = "The database could not be prepared; the app started without a data layer")]
    private static partial void LogStartupFailed(ILogger logger, Exception exception);

    /// <summary>App and device context, written once at the top of each session's log entries.</summary>
    private static string SessionHeader()
    {
        // DeviceInfo.Name is the name the user gave the device ("My iPhone"), so it is not one of these.
        string[] lines =
        [
            $"App: Crafting Calculator {AppInfo.Current.VersionString} (build {AppInfo.Current.BuildString})",
            $"OS: {DeviceInfo.Current.Platform} {DeviceInfo.Current.VersionString}",
            $"Device: {DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model} ({DeviceInfo.Current.Idiom})",
#if DEBUG
            "Redaction: off (Debug build)"
#else
            "Redaction: on"
#endif
        ];

        return string.Join(Environment.NewLine, lines);
    }

#if !DEBUG
    /// <summary>The folders whose contents are the user's, and so never appear in the log by name.</summary>
    private static IReadOnlyList<string> RedactedRoots() =>
    [
        FileSystem.AppDataDirectory,
        FileSystem.CacheDirectory,
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        // Windows is unpackaged, so the account name is in every one of the paths above; a file the user
        // picked to import is somewhere else under the same profile.
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    ];
#endif

    private static string GetDatabasePath()
        => Path.Combine(FileSystem.AppDataDirectory, "CraftingCalculator.db3");

    // A subdirectory of its own, so the Android backup rules can exclude the logs while the database beside
    // them stays backed up.
    private static string GetLogsPath()
    {
#if IOS
        // Documents is what the Files app shows, so a user can hand over a log without a cable. Excluded from
        // iCloud: diagnostics are the device's, and must not ride along in the user's backup.
        string logsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Logs");
        Directory.CreateDirectory(logsPath);
        using NSUrl url = NSUrl.FromFilename(logsPath);
        _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));

        return logsPath;
#else
        return Path.Combine(FileSystem.AppDataDirectory, "Logs");
#endif
    }

    // Resolved on every launch and never stored: the iOS sandbox path carries a container id that changes when
    // the app is reinstalled.
    private static string GetExportsPath()
#if IOS
        // Documents is what the Files app shows under On My iPhone once Info.plist enables file sharing;
        // AppDataDirectory is Library, which no user can reach.
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Exports");
#else
        => Path.Combine(FileSystem.AppDataDirectory, "Exports");
#endif
}
