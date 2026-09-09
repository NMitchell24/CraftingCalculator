using AwesomeAssertions;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator.Infrastructure.UnitTests;

[TestFixture]
public class MigrationTests
{
    [Test]
    public async Task Migrate_OnAnEmptyFile_CreatesTheSchemaAndSeedsExactlyTheAllCategory()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = new(options))
            {
                List<Category> categories = await context.Categories.ToListAsync();
                categories.Should().ContainSingle();
                categories[0].Id.Should().Be(DatabaseSeedConstants.AllCategoryId);
                categories[0].Name.Should().Be(CategoryModel.All);

                (await context.Components.CountAsync()).Should().Be(0);
                (await context.Blueprints.CountAsync()).Should().Be(0);
                (await context.Favorites.CountAsync()).Should().Be(0);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    /// <summary>
    /// The rename migration must carry an existing install's data across rather than rebuild the
    /// schema: "dotnet ef migrations add" scaffolds these renames as DropTable/CreateTable pairs,
    /// which would silently empty every table. This walks a database up to the pre-rename
    /// migration, fills it through the old schema's names, then migrates to the current model and
    /// reads the same rows back - including the values behind every renamed foreign key column.
    /// </summary>
    [Test]
    public async Task Migrate_FromThePreRenameSchema_PreservesExistingData()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.GetService<IMigrator>().MigrateAsync("InsertSeedData");

                // Deliberately raw SQL against the pre-rename table and column names: the entity
                // classes no longer describe this schema, which is the whole point of the test.
                await context.Database.ExecuteSqlRawAsync(
                    """
                    INSERT INTO RecipeFilters (Id, Name, Description) VALUES (2, 'Furniture', 'Things to sit on');
                    INSERT INTO Ingredients (Id, Name, Description, Cost) VALUES (1, 'Oak Plank', 'Sawn oak', 2.5);
                    INSERT INTO Ingredients (Id, Name, Description, Cost) VALUES (2, 'Iron Nail', 'Forged', 0.25);
                    INSERT INTO Recipes (Id, Name, Description, Value, FilterId) VALUES (1, 'Table Leg', 'One leg', 4.0, 2);
                    INSERT INTO Recipes (Id, Name, Description, Value, FilterId) VALUES (2, 'Table', 'Four legs', 30.0, 2);
                    INSERT INTO RecipeIngredients (Id, RecipeId, IngredientId, Quantity) VALUES (1, 1, 1, 3);
                    INSERT INTO RecipeIngredients (Id, RecipeId, IngredientId, Quantity) VALUES (2, 2, 2, 12);
                    INSERT INTO RecipeChildren (Id, ParentRecipeId, ChildRecipeId, Quantity) VALUES (1, 2, 1, 4);
                    INSERT INTO Favorites (Id, Name) VALUES (1, 'Dining set');
                    INSERT INTO FavoriteRecipes (Id, FavoriteId, RecipeId, Quantity) VALUES (1, 1, 2, 2);
                    """);
            }

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = new(options))
            {
                Blueprint table = await context.Blueprints
                    .Include(b => b.Category)
                    .Include(b => b.Components).ThenInclude(bc => bc.Component)
                    .Include(b => b.Children).ThenInclude(bc => bc.Child)
                    .SingleAsync(b => b.Name == "Table");

                // Renamed column Recipes.FilterId -> Blueprints.CategoryId still points at the row
                // that moved from RecipeFilters to Categories.
                table.Category!.Name.Should().Be("Furniture");
                table.Value.Should().Be(30.0);

                // RecipeIngredients.IngredientId -> BlueprintComponents.ComponentId
                BlueprintComponent nails = table.Components.Should().ContainSingle().Subject;
                nails.Component.Name.Should().Be("Iron Nail");
                nails.Component.Cost.Should().Be(0.25);
                nails.Quantity.Should().Be(12);

                // RecipeChildren.ParentRecipeId/ChildRecipeId ->
                // BlueprintChildren.ParentBlueprintId/ChildBlueprintId
                BlueprintChild leg = table.Children.Should().ContainSingle().Subject;
                leg.Child.Name.Should().Be("Table Leg");
                leg.Quantity.Should().Be(4);

                // FavoriteRecipes.RecipeId -> FavoriteBlueprints.BlueprintId
                Favorite favorite = await context.Favorites
                    .Include(f => f.FavoriteBlueprints).ThenInclude(fb => fb.Blueprint)
                    .SingleAsync();
                favorite.Name.Should().Be("Dining set");
                favorite.FavoriteBlueprints.Should().ContainSingle()
                    .Which.Blueprint.Name.Should().Be("Table");

                // The seeded "All" row and the user's own category both survive.
                (await context.Categories.CountAsync()).Should().Be(2);
                (await context.Components.CountAsync()).Should().Be(2);
                (await context.Blueprints.CountAsync()).Should().Be(2);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    /// <summary>
    /// Yield arrived after the app shipped, so every blueprint written before it must come back as the
    /// 1-per-craft default rather than 0, which BlueprintProcessor.CraftsFor rejects.
    /// </summary>
    [Test]
    public async Task Migrate_FromTheSchemaBeforeYield_DefaultsExistingBlueprintsToOne()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.GetService<IMigrator>().MigrateAsync("RenameToCraftingVocabulary");

                // Raw SQL because the entity no longer describes a Blueprints table without a Yield column.
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Blueprints (Id, Name, Description, Value) VALUES (1, 'Table', 'Four legs', 30.0);");
            }

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = new(options))
            {
                Blueprint table = await context.Blueprints.SingleAsync();
                table.Name.Should().Be("Table");
                table.Yield.Should().Be(1);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    /// <summary>
    /// Production time arrived after yield, so blueprints and components written before it must come
    /// back as instant rather than as some non-zero tick count read out of an unwritten column.
    /// </summary>
    [Test]
    public async Task Migrate_FromTheSchemaBeforeProductionTime_DefaultsExistingRowsToInstant()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.GetService<IMigrator>().MigrateAsync("AddBlueprintYield");

                // Raw SQL because neither entity describes a table without a ProductionTime column now.
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Blueprints (Id, Name, Description, Value, Yield) VALUES (1, 'Soup', 'Warm', 30.0, 1);");
                await context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Components (Id, Name, Description, Cost) VALUES (1, 'Potato', 'Starchy', 2.0);");
            }

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = new(options))
            {
                Blueprint soup = await context.Blueprints.SingleAsync();
                soup.Name.Should().Be("Soup");
                soup.ProductionTime.Should().Be(TimeSpan.Zero);

                Component potato = await context.Components.SingleAsync();
                potato.Name.Should().Be("Potato");
                potato.ProductionTime.Should().Be(TimeSpan.Zero);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    private static string NewDbPath() =>
        Path.Combine(Path.GetTempPath(), $"crafting_migration_{Guid.NewGuid():N}.db3");

    private static DbContextOptions<CraftingDataContext> OptionsFor(string dbPath) =>
        new DbContextOptionsBuilder<CraftingDataContext>().UseSqlite($"Filename={dbPath}").Options;

    private static void Cleanup(string dbPath)
    {
        SqliteConnection.ClearAllPools();
        if (File.Exists(dbPath)) File.Delete(dbPath);
    }
}
