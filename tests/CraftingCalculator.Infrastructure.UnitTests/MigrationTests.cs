using AwesomeAssertions;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using CategoryModel = CraftingCalculator.Domain.Models.Category;

namespace CraftingCalculator.Infrastructure.UnitTests;

[TestFixture]
public class MigrationTests
{
    [Test]
    public async Task Migrate_OnAnEmptyFile_CreatesTheSchemaAndSeedsExactlyTheAllCategory()
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
                List<Category> categorys = await context.Categorys.ToListAsync();
                categorys.Should().ContainSingle();
                categorys[0].Id.Should().Be(DatabaseSeedConstants.AllCategoryId);
                categorys[0].Name.Should().Be(CategoryModel.ALL);

                (await context.Components.CountAsync()).Should().Be(0);
                (await context.Blueprints.CountAsync()).Should().Be(0);
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
