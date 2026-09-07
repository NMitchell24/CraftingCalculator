using AwesomeAssertions;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RecipeFilterModel = CraftingCalculator.Domain.Models.RecipeFilter;

namespace CraftingCalculator.Infrastructure.UnitTests;

[TestFixture]
public class MigrationTests
{
    [Test]
    public async Task Migrate_OnAnEmptyFile_CreatesTheSchemaAndSeedsExactlyTheAllFilter()
    {
        string dbPath = Path.Combine(Path.GetTempPath(), $"crafting_migration_{Guid.NewGuid():N}.db3");
        try
        {
            DbContextOptions<CraftingDataContext> options = new DbContextOptionsBuilder<CraftingDataContext>()
                .UseSqlite($"Filename={dbPath}")
                .Options;

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = new(options))
            {
                List<RecipeFilter> filters = await context.RecipeFilters.ToListAsync();
                filters.Should().ContainSingle();
                filters[0].Id.Should().Be(DatabaseSeedConstants.AllFilterId);
                filters[0].Name.Should().Be(RecipeFilterModel.ALL);

                (await context.Ingredients.CountAsync()).Should().Be(0);
                (await context.Recipes.CountAsync()).Should().Be(0);
                (await context.Favorites.CountAsync()).Should().Be(0);
            }
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(dbPath)) File.Delete(dbPath);
        }
    }
}
