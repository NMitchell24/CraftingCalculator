using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IComponentService, ComponentService>();
        services.AddScoped<IRecipeFilterService, RecipeFilterService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IDatabaseAdminService, DatabaseAdminService>();
        services.AddScoped<ILibraryService, LibraryService>();

        return services;
    }
}
