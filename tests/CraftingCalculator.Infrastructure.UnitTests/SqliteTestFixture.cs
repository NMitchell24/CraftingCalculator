using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// A real (never <c>UseInMemoryDatabase</c>) SQLite database shared by an in-memory connection
/// string, so foreign key enforcement and cascade delete - the mechanisms these tests exist to
/// verify - actually run. A dedicated connection is held open for the fixture's lifetime purely to
/// keep the shared in-memory database alive; every <see cref="IDbContextFactory{TContext}"/> call
/// opens its own separate connection against the same connection string, matching how
/// <see cref="CraftingDataContext"/> is used in production.
/// </summary>
public sealed class SqliteTestFixture : IDisposable
{
    // A bare ":memory:" database is NOT name-addressable across connections even under
    // Cache=Shared - each connection using the literal ":memory:" string gets its own anonymous
    // database. Mode=Memory with an explicit Data Source name is the documented way to give
    // multiple connections a shared in-memory database. The name is unique per fixture instance so
    // parallel test fixtures don't see each other's data.
    private readonly string _connectionString = $"Data Source=crafting-test-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

    private readonly SqliteConnection _keeperConnection;

    public IDbContextFactory<CraftingDataContext> Factory { get; }

    public SqliteTestFixture()
    {
        _keeperConnection = new SqliteConnection(_connectionString);
        _keeperConnection.Open();

        using (CraftingDataContext context = CreateContext())
        {
            context.Database.Migrate();
        }

        Factory = new TestContextFactory(_connectionString);
    }

    public void Dispose() => _keeperConnection.Dispose();

    private CraftingDataContext CreateContext()
    {
        DbContextOptions<CraftingDataContext> options = new DbContextOptionsBuilder<CraftingDataContext>()
            .UseSqlite(_connectionString)
            .Options;
        return new CraftingDataContext(options);
    }

    private sealed class TestContextFactory(string connectionString) : IDbContextFactory<CraftingDataContext>
    {
        public CraftingDataContext CreateDbContext()
        {
            DbContextOptions<CraftingDataContext> options = new DbContextOptionsBuilder<CraftingDataContext>()
                .UseSqlite(connectionString)
                .Options;
            return new CraftingDataContext(options);
        }
    }
}
