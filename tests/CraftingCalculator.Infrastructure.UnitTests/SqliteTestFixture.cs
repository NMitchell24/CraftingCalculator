using CraftingCalculator.Application.Common.Interfaces;
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
    /// <summary>Id the AddDatasets migration gives the dataset it files every pre-existing record into.</summary>
    public const int DefaultDatasetId = 1;

    // A bare ":memory:" database is NOT name-addressable across connections even under
    // Cache=Shared - each connection using the literal ":memory:" string gets its own anonymous
    // database. Mode=Memory with an explicit Data Source name is the documented way to give
    // multiple connections a shared in-memory database. The name is unique per fixture instance so
    // parallel test fixtures don't see each other's data.
    private readonly string _connectionString = $"Data Source=crafting-test-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

    private readonly SqliteConnection _keeperConnection;
    private readonly TestSelectedDatasetState _selectedDataset = new(DefaultDatasetId);

    /// <summary>
    /// Contexts already scoped to the selected dataset, for tests that read and write entities
    /// directly rather than through a DAO.
    /// </summary>
    public IDbContextFactory<CraftingDataContext> Factory { get; }

    /// <summary>What every DAO but <see cref="DAO.Impl.DatasetDAO"/> takes.</summary>
    public DatasetScopedContextFactory DatasetFactory { get; }

    /// <summary>
    /// Unscoped contexts, as <see cref="DAO.Impl.DatasetDAO"/> uses. Reads through this see every
    /// dataset's rows, which is what makes it the way to assert on a dataset that is not selected.
    /// </summary>
    public IDbContextFactory<CraftingDataContext> RawFactory { get; }

    public SqliteTestFixture()
    {
        _keeperConnection = new SqliteConnection(_connectionString);
        _keeperConnection.Open();

        RawFactory = new TestContextFactory(_connectionString, selectedDataset: null);

        using (CraftingDataContext context = RawFactory.CreateDbContext())
        {
            context.Database.Migrate();
        }

        Factory = new TestContextFactory(_connectionString, _selectedDataset);
        DatasetFactory = new DatasetScopedContextFactory(RawFactory, _selectedDataset);
    }

    /// <summary>Points <see cref="Factory"/> and <see cref="DatasetFactory"/> at another dataset.</summary>
    public void SelectDataset(int id) => _selectedDataset.Set(id);

    public void Dispose() => _keeperConnection.Dispose();

    private sealed class TestContextFactory(string connectionString, ISelectedDatasetState? selectedDataset)
        : IDbContextFactory<CraftingDataContext>
    {
        public CraftingDataContext CreateDbContext()
        {
            DbContextOptions<CraftingDataContext> options = new DbContextOptionsBuilder<CraftingDataContext>()
                .UseSqlite(connectionString)
                .Options;

            return new CraftingDataContext(options)
            {
                // Left at 0 for the raw factory, which matches a context that has never been scoped.
                DatasetId = selectedDataset?.Id ?? 0
            };
        }
    }

    /// <summary>Stands in for the persisted selection; these tests have no preference store.</summary>
    private sealed class TestSelectedDatasetState(int id) : ISelectedDatasetState
    {
        public int Id { get; private set; } = id;

        public void Set(int id) => Id = id;
    }
}
