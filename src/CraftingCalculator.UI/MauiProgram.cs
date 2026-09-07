using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Infrastructure;
using CraftingCalculator.UI.Platform;
using CraftingCalculator.UI.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace CraftingCalculator.UI;

public static class MauiProgram
{
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

        CraftingCalculator.Application.DependencyInjection.AddApplicationServices(builder.Services);
        builder.Services.AddDatabaseServices(GetDatabasePath());

        builder.Services.AddSingleton<IClipboardService, ClipboardService>();
        builder.Services.AddSingleton<IPreferenceStore, PreferenceStore>();

        builder.Services.AddScoped<CalculatorState>();
        builder.Services.AddScoped<AppBarState>();
        builder.Services.AddScoped<ThemeState>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        MauiApp app = builder.Build();

        // The DB must exist (and be migrated) before any page loads.
        using (IServiceScope scope = app.Services.CreateScope())
        {
            IDbContextFactory<CraftingDataContext> contextFactory =
                scope.ServiceProvider.GetRequiredService<IDbContextFactory<CraftingDataContext>>();
            using CraftingDataContext context = contextFactory.CreateDbContext();
            context.Database.Migrate();
        }

        return app;
    }

    private static string GetDatabasePath()
        => Path.Combine(FileSystem.AppDataDirectory, "CraftingCalculator.db3");
}
