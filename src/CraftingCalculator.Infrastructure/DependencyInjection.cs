using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Infrastructure.DAO.Impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the EF Core context factory and DAO implementations against the SQLite database at
    /// <paramref name="dbPath"/>.
    /// </summary>
    /// <remarks>
    /// Registers <see cref="CraftingDataContext"/> only via a pooled <see cref="IDbContextFactory{TContext}"/>,
    /// never the context itself: <c>BlazorWebView</c> creates exactly one <see cref="IServiceScope"/>
    /// for the whole app session, so a scoped <see cref="CraftingDataContext"/> would live for the
    /// entire session instead of one operation.
    /// </remarks>
    public static void AddDatabaseServices(this IServiceCollection services, string dbPath)
    {
        services.AddPooledDbContextFactory<CraftingDataContext>(options =>
        {
            options.UseSqlite($"Filename={dbPath}");
#if DEBUG
            options.LogTo(msg => System.Diagnostics.Debug.WriteLine(msg), LogLevel.Information)
                .EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IComponentDAO, ComponentDAO>();
        services.AddScoped<IBlueprintFilterDAO, BlueprintFilterDAO>();
        services.AddScoped<IBlueprintDAO, BlueprintDAO>();
        services.AddScoped<IBlueprintFavoritesDAO, BlueprintFavoritesDAO>();
        services.AddScoped<IDatabaseAdminDAO, DatabaseAdminDAO>();
    }
}
