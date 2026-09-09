using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBlueprintService, BlueprintService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IDatabaseAdminService, DatabaseAdminService>();
        services.AddScoped<IDatasetService, DatasetService>();
        services.AddScoped<IHelpService, HelpService>();

        return services;
    }
}
