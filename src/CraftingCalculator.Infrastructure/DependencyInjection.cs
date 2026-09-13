using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Infrastructure.DAO.Impl;
using CraftingCalculator.Infrastructure.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CraftingCalculator.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
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
        public void AddDatabaseServices(string dbPath)
        {
            services.AddPooledDbContextFactory<CraftingDataContext>(options =>
            {
                options.UseSqlite($"Filename={dbPath}");
#if DEBUG
                options.LogTo(msg => System.Diagnostics.Debug.WriteLine(msg), LogLevel.Information)
                    .EnableSensitiveDataLogging();
#endif
            });

            // Every DAO but DatasetDAO reaches the database through this, which is what scopes them to
            // the selected dataset.
            services.AddScoped<DatasetScopedContextFactory>();

            services.AddScoped<IDatasetDAO, DatasetDAO>();
            services.AddScoped<IComponentDAO, ComponentDAO>();
            services.AddScoped<ICategoryDAO, CategoryDAO>();
            services.AddScoped<IBlueprintDAO, BlueprintDAO>();
            services.AddScoped<IBlueprintFavoritesDAO, BlueprintFavoritesDAO>();
            services.AddScoped<IDatabaseAdminDAO, DatabaseAdminDAO>();
        }

        /// <summary>Registers the export file store against the folder at <paramref name="exportsDirectory"/>.</summary>
        public void AddExportFileServices(string exportsDirectory) =>
            services.AddSingleton<IExportFileStore>(new ExportFileStore(exportsDirectory));
    }
}
