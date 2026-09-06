using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CraftingCalculator.Infrastructure;

/// <summary>
/// Design-time factory for EF Core migrations. Only used by dotnet ef tools; not at runtime.
/// </summary>
// ReSharper disable once UnusedType.Global
public class CraftingDataContextFactory : IDesignTimeDbContextFactory<CraftingDataContext>
{
    public CraftingDataContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CraftingDataContext> optionsBuilder = new();

        // The actual path doesn't matter for migrations - only a temporary SQLite database is needed.
        optionsBuilder.UseSqlite("Data Source=:memory:");

        return new CraftingDataContext(optionsBuilder.Options);
    }
}
