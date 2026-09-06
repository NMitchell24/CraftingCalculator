using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Infrastructure;
using CraftingCalculator.UI.Platform;
using CraftingCalculator.UI.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        builder.Services.AddMudServices();

        CraftingCalculator.Application.DependencyInjection.AddApplicationServices(builder.Services);
        builder.Services.AddDatabaseServices(GetDatabasePath());

        builder.Services.AddSingleton<IClipboardService, ClipboardService>();
        builder.Services.AddSingleton<IPreferenceStore, PreferenceStore>();

        builder.Services.AddScoped<CalculatorState>();
        builder.Services.AddScoped<AppBarState>();

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
