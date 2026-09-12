using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Singleton: startup resolves the selection in its own scope, not the one BlazorWebView
        // holds for the session.
        services.AddSingleton<ISelectedDatasetState, SelectedDatasetState>();

        services.AddScoped<IDatasetService, DatasetService>();
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBlueprintService, BlueprintService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IDatabaseAdminService, DatabaseAdminService>();
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<IHelpService, HelpService>();

        return services;
    }
}
