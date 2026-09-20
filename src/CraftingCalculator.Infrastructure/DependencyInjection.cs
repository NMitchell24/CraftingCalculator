using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Infrastructure.DAO.Impl;
using CraftingCalculator.Infrastructure.Files;
using CraftingCalculator.Infrastructure.Logging;
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

    extension(ILoggingBuilder logging)
    {
        /// <summary>
        /// Writes the log to files under <paramref name="logDirectory"/> verbatim, and registers it as
        /// <see cref="IDiagnosticLog"/>. For development only: an exception message is recorded exactly as the
        /// framework wrote it, which can include a record the user named.
        /// </summary>
        public void AddFileLogging(string logDirectory) =>
            AddFileLogging(logging, new FileLoggerProvider(logDirectory, redactor: null));

        /// <summary>
        /// Writes the log to files under <paramref name="logDirectory"/> with everything under
        /// <paramref name="redactedRoots"/>, and every value the framework quotes, stripped out; and registers
        /// it as <see cref="IDiagnosticLog"/>.
        /// </summary>
        /// <param name="logDirectory">The folder the log files live in.</param>
        /// <param name="redactedRoots">The folders holding the user's files, such as app data and Documents.</param>
        public void AddRedactedFileLogging(string logDirectory, IReadOnlyList<string> redactedRoots) =>
            AddFileLogging(logging, new FileLoggerProvider(logDirectory, new LogRedactor(redactedRoots)));
    }

    // Two entry points rather than one with a bool: a flag that forks the whole formatter is two methods. The
    // shared half sits outside the extension block, which cannot hold a private member.
    private static void AddFileLogging(ILoggingBuilder logging, FileLoggerProvider provider)
    {
        logging.AddProvider(provider);

        // Framework categories log at Information and would fill the size-capped file on their own.
        logging.AddFilter<FileLoggerProvider>("Microsoft", LogLevel.Warning);

        // EF is off entirely, in both configurations: AddDatabaseServices turns on EnableSensitiveDataLogging
        // in Debug, and a failed command then logs its parameter values - the user's data - at Error. The
        // debugger output window still gets them.
        logging.AddFilter<FileLoggerProvider>("Microsoft.EntityFrameworkCore", LogLevel.None);

        logging.Services.AddSingleton<IDiagnosticLog>(provider);
    }
}
