using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace CraftingCalculator.Infrastructure.UnitTests;

[TestFixture]
public class MigrationTests
{
    /// <summary>Id the InsertSeedData migration gave the "All" category row it used to create.</summary>
    private const int SeededAllCategoryId = 1;

    /// <summary>Id the AddDatasets migration gives the dataset it files every pre-existing row into.</summary>
    private const int DefaultDatasetId = 1;

    [Test]
    public async Task Migrate_OnAnEmptyFile_CreatesTheSchemaWithNoRows()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = DefaultDatasetContext(options))
            {
                (await context.Categories.CountAsync()).Should().Be(0);
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

            await using (CraftingDataContext context = DefaultDatasetContext(options))
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

                // The user's own category survives; the seeded "All" row is the one RemoveSeededAllCategory drops.
                (await context.Categories.CountAsync()).Should().Be(1);
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

            await using (CraftingDataContext context = DefaultDatasetContext(options))
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

            await using (CraftingDataContext context = DefaultDatasetContext(options))
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

    /// <summary>
    /// The seeded "All" row was a filter sentinel that no screen ever rendered, so it is removed
    /// rather than left behind as a category the user can see but never created. A blueprint pointing
    /// at it comes back uncategorized rather than orphaned.
    /// </summary>
    [Test]
    public async Task Migrate_FromTheSchemaWithTheSeededAllCategory_RemovesItAndUncategorizesItsBlueprints()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.GetService<IMigrator>().MigrateAsync("AddProductionTime");

                // Raw SQL, though this migration changes no schema: the entities now carry a DatasetId
                // that AddDatasets has not added yet at this point, so writing through the model would
                // insert a column the table does not have.
                const int furnitureId = 2;
                await context.Database.ExecuteSqlRawAsync(
                    $"""
                     INSERT INTO Categories (Id, Name, Description) VALUES ({furnitureId}, 'Furniture', 'Things to sit on');
                     INSERT INTO Blueprints (Id, Name, Description, Value, Yield, CategoryId) VALUES (1, 'Table', 'Four legs', 30.0, 1, {SeededAllCategoryId});
                     INSERT INTO Blueprints (Id, Name, Description, Value, Yield, CategoryId) VALUES (2, 'Chair', 'One seat', 12.0, 1, {furnitureId});
                     """);
            }

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = DefaultDatasetContext(options))
            {
                Category furniture = (await context.Categories.ToListAsync()).Should().ContainSingle().Subject;
                furniture.Name.Should().Be("Furniture");

                (await context.Blueprints.SingleAsync(blueprint => blueprint.Name == "Table")).CategoryId.Should().BeNull();
                (await context.Blueprints.SingleAsync(blueprint => blueprint.Name == "Chair")).CategoryId.Should().Be(furniture.Id);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    /// <summary>
    /// Datasets arrived after the app shipped, so every record written before them has to land in one
    /// dataset rather than in dataset 0, which matches no row - the scaffolded migration's
    /// defaultValue of 0 would have left an upgraded install showing an empty app and failing the new
    /// foreign key.
    /// </summary>
    [Test]
    public async Task Migrate_FromTheSchemaBeforeDatasets_FilesExistingRecordsUnderDefault()
    {
        string dbPath = NewDbPath();
        try
        {
            DbContextOptions<CraftingDataContext> options = OptionsFor(dbPath);

            await using (CraftingDataContext context = new(options))
            {
                await context.GetService<IMigrator>().MigrateAsync("AddComponentCategory");

                // Raw SQL because no entity describes these tables without a DatasetId column now.
                await context.Database.ExecuteSqlRawAsync(
                    """
                    INSERT INTO Categories (Id, Name, Description) VALUES (2, 'Ores', 'Dug up');
                    INSERT INTO Components (Id, Name, Description, Cost, ProductionTime, CategoryId) VALUES (1, 'Copper', 'Ore', 2.0, 0, 2);
                    INSERT INTO Blueprints (Id, Name, Description, Value, Yield, ProductionTime, CategoryId) VALUES (1, 'Bronze', 'Alloy', 9.0, 1, 0, 2);
                    INSERT INTO BlueprintComponents (Id, BlueprintId, ComponentId, Quantity) VALUES (1, 1, 1, 2);
                    INSERT INTO Favorites (Id, Name) VALUES (1, 'Starter kit');
                    INSERT INTO FavoriteBlueprints (Id, FavoriteId, BlueprintId, Quantity) VALUES (1, 1, 1, 4);
                    """);
            }

            await using (CraftingDataContext context = new(options))
            {
                await context.Database.MigrateAsync();
            }

            await using (CraftingDataContext context = DefaultDatasetContext(options))
            {
                // One dataset, named so the upgrading user has something to rename rather than a blank.
                Dataset dataset = await context.Datasets.SingleAsync();
                dataset.Id.Should().Be(DefaultDatasetId);
                dataset.Name.Should().Be("Default");

                // Every record reads back through the dataset filter, which is only true if the column
                // was backfilled with this dataset's id.
                Blueprint bronze = await context.Blueprints
                    .Include(b => b.Category)
                    .Include(b => b.Components).ThenInclude(bc => bc.Component)
                    .SingleAsync();
                bronze.Name.Should().Be("Bronze");
                bronze.DatasetId.Should().Be(DefaultDatasetId);
                bronze.Category!.Name.Should().Be("Ores");
                bronze.Components.Should().ContainSingle().Which.Component.Name.Should().Be("Copper");

                Favorite favorite = await context.Favorites
                    .Include(f => f.FavoriteBlueprints)
                    .SingleAsync();
                favorite.Name.Should().Be("Starter kit");
                favorite.FavoriteBlueprints.Should().ContainSingle().Which.Quantity.Should().Be(4);

                (await context.Categories.SingleAsync()).DatasetId.Should().Be(DefaultDatasetId);
                (await context.Components.SingleAsync()).DatasetId.Should().Be(DefaultDatasetId);
                favorite.DatasetId.Should().Be(DefaultDatasetId);
            }
        }
        finally
        {
            Cleanup(dbPath);
        }
    }

    /// <summary>
    /// A context scoped to the dataset AddDatasets files every pre-existing row into. Reads through an
    /// unscoped context come back empty: every record type carries a dataset query filter, and a
    /// hand-built context has no DatasetId until something sets one.
    /// </summary>
    private static CraftingDataContext DefaultDatasetContext(DbContextOptions<CraftingDataContext> options) =>
        new(options) { DatasetId = DefaultDatasetId };

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
