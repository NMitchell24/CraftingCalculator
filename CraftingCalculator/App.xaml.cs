using System;
using System.IO;
using System.Windows;
using CraftingCalculator.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    // Sibling namespace CraftingCalculator.Application means the bare identifier "Application" here
    // resolves to that namespace, not System.Windows.Application - both the base class and every
    // reference to that namespace below must stay fully qualified (CS0118 otherwise).
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            ServiceCollection services = new();
            CraftingCalculator.Application.DependencyInjection.AddApplicationServices(services);

            string dbDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CraftingCalculator");
            Directory.CreateDirectory(dbDirectory);
            services.AddDatabaseServices(Path.Combine(dbDirectory, "CraftingCalculator.db"));

            Services = services.BuildServiceProvider();

            // Must ensure the DB exists (and is migrated) before any window opens.
            using CraftingDataContext context = Services
                .GetRequiredService<IDbContextFactory<CraftingDataContext>>()
                .CreateDbContext();
            context.Database.Migrate();

            base.OnStartup(e);
        }
    }
}
