using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// The isolation the whole multiple-datasets feature rests on: a DAO sees only the selected dataset's
/// rows, and a record it writes is filed under that dataset. Nothing in any DAO says so - it comes from
/// the query filters and the insert stamping in <see cref="CraftingDataContext"/> - so this is where
/// that contract is actually pinned down.
/// </summary>
[TestFixture]
public class DatasetScopingTests
{
    private SqliteTestFixture _fixture = null!;
    private DatasetDAO _datasetDAO = null!;
    private CategoryDAO _categoryDAO = null!;
    private ComponentDAO _componentDAO = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private BlueprintFavoritesDAO _favoritesDAO = null!;
    private int _secondDatasetId;

    [SetUp]
    public async Task SetUp()
    {
        _fixture = new SqliteTestFixture();
        _datasetDAO = new DatasetDAO(_fixture.RawFactory);
        _categoryDAO = new CategoryDAO(_fixture.DatasetFactory);
        _componentDAO = new ComponentDAO(_fixture.DatasetFactory);
        _blueprintDAO = new BlueprintDAO(_fixture.DatasetFactory);
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.DatasetFactory);

        _secondDatasetId = (await _datasetDAO.AddAsync("Rust")).Id;
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task RecordsWrittenInOneDataset_AreInvisibleFromAnother()
    {
        await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Starter kit" }, []);

        _fixture.SelectDataset(_secondDatasetId);

        (await _categoryDAO.GetAllAsync()).Should().BeEmpty();
        (await _componentDAO.GetAllAsync()).Should().BeEmpty();
        (await _blueprintDAO.GetSummariesAsync()).Should().BeEmpty();
        (await _favoritesDAO.GetAllAsync()).Should().BeEmpty();
    }

    [Test]
    public async Task CountsSeeOnlyTheSelectedDataset()
    {
        await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });

        (await _categoryDAO.CountAsync()).Should().Be(1);
        (await _componentDAO.CountAsync()).Should().Be(1);
        (await _blueprintDAO.CountAsync()).Should().Be(1);

        _fixture.SelectDataset(_secondDatasetId);

        (await _categoryDAO.CountAsync()).Should().Be(0);
        (await _componentDAO.CountAsync()).Should().Be(0);
        (await _blueprintDAO.CountAsync()).Should().Be(0);
        (await _blueprintDAO.GetByIdAsync(bronze.Id)).Should().BeNull();
    }

    [Test]
    public async Task ARecordIsFiledUnderTheSelectedDataset()
    {
        _fixture.SelectDataset(_secondDatasetId);
        ComponentModel saved = await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur" });

        await using CraftingDataContext raw = _fixture.RawFactory.CreateDbContext();
        Component stored = await raw.Components.IgnoreQueryFilters().SingleAsync(c => c.Id == saved.Id);

        stored.DatasetId.Should().Be(_secondDatasetId);
    }

    /// <summary>
    /// A record reached only through another record's navigation is filed too, and saving leaves the context's change
    /// detection as it found it for whatever the caller does next.
    /// </summary>
    [Test]
    public async Task ARecordAddedThroughANavigation_IsFiledUnderTheSelectedDataset()
    {
        _fixture.SelectDataset(_secondDatasetId);
        await using CraftingDataContext context = await _fixture.DatasetFactory.CreateAsync();

        Blueprint arrow = new() { Name = "Wooden Arrow", Description = "" };
        context.Blueprints.Add(arrow);
        await context.SaveChangesAsync();

        arrow.Components.Add(new BlueprintComponent
        {
            Component = new Component { Name = "Wood", Description = "" },
            Quantity = 25
        });
        await context.SaveChangesAsync();

        context.ChangeTracker.AutoDetectChangesEnabled.Should().BeTrue();

        await using CraftingDataContext raw = await _fixture.RawFactory.CreateDbContextAsync();
        Component stored = await raw.Components.IgnoreQueryFilters().SingleAsync(c => c.Name == "Wood");
        stored.DatasetId.Should().Be(_secondDatasetId);
    }

    /// <summary>
    /// Nothing in the schema stops a link row from naming a record in another dataset, and a blueprint read resolves
    /// every link it loads. A link like that is dropped on read, the way a cyclic one is, rather than failing every
    /// screen that reads the blueprint.
    /// </summary>
    [Test]
    public async Task ALinkToAnotherDatasetsRecord_IsDroppedOnRead()
    {
        _fixture.SelectDataset(_secondDatasetId);
        ComponentModel sulfur = await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur" });
        BlueprintModel gunpowder = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Gunpowder" });

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        ComponentModel copper = await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        BlueprintModel bronze = new() { Name = "Bronze" };
        bronze.Components.Add(copper, 2);
        bronze.Components.Add(sulfur, 1);
        bronze.ChildBlueprints.Add(gunpowder, 3);
        bronze = await _blueprintDAO.SaveAsync(bronze);

        BlueprintModel loaded = (await _blueprintDAO.GetByIdAsync(bronze.Id))!;

        loaded.Components.ComponentList.Should().ContainSingle().Which.Component.Name.Should().Be("Copper");
        loaded.ChildBlueprints.BlueprintList.Should().BeEmpty();
    }

    /// <summary>
    /// A path that leaves the dataset and comes back in is not part of the blueprint: the other dataset's rows are
    /// not this blueprint's parts, whatever they point at.
    /// </summary>
    [Test]
    public async Task APathThroughAnotherDataset_IsDroppedOnATreeRead()
    {
        BlueprintModel ingot = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Ingot" });

        _fixture.SelectDataset(_secondDatasetId);
        BlueprintModel foreign = new() { Name = "Foreign" };
        foreign.ChildBlueprints.Add(ingot, 1);
        foreign = await _blueprintDAO.SaveAsync(foreign);

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        BlueprintModel sword = new() { Name = "Sword" };
        sword.ChildBlueprints.Add(foreign, 1);
        sword = await _blueprintDAO.SaveAsync(sword);

        BlueprintModel loaded = (await _blueprintDAO.GetByIdAsync(sword.Id))!;

        loaded.ChildBlueprints.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public async Task APathThroughAnotherDataset_IsDroppedOnAnAncestorRead()
    {
        BlueprintModel ingot = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Ingot" });

        _fixture.SelectDataset(_secondDatasetId);
        BlueprintModel foreign = new() { Name = "Foreign" };
        foreign.ChildBlueprints.Add(ingot, 1);
        foreign = await _blueprintDAO.SaveAsync(foreign);

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        BlueprintModel sword = new() { Name = "Sword" };
        sword.ChildBlueprints.Add(foreign, 1);
        await _blueprintDAO.SaveAsync(sword);

        (await _blueprintDAO.GetAncestorIdsAsync(ingot.Id)).Should().BeEmpty();
    }

    [Test]
    public async Task AFavoriteRowNamingAnotherDatasetsBlueprint_IsDroppedOnRead()
    {
        _fixture.SelectDataset(_secondDatasetId);
        BlueprintModel foreign = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Foreign" });

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });
        BlueprintFavorite favorite = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Mixed" },
            [new BlueprintQuantity(bronze, 2), new BlueprintQuantity(foreign, 1)]);

        List<BlueprintQuantity> quantities = await _favoritesDAO.GetBlueprintQuantitiesAsync(favorite.Id);

        quantities.Should().ContainSingle().Which.Blueprint.Name.Should().Be("Bronze");
    }

    [Test]
    public async Task TwoDatasetsCanHoldRecordsOfTheSameName()
    {
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });

        _fixture.SelectDataset(_secondDatasetId);
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 7 });

        // The point of datasets: the same word means a different thing in a different game, and neither
        // record has to be renamed to keep them apart.
        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Cost.Should().Be(7);

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Cost.Should().Be(1);
    }

    [Test]
    public async Task ABlueprintsComponentGraphIsScopedToItsOwnDataset()
    {
        ComponentModel copper = await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper", Cost = 2 });
        BlueprintModel bar = new() { Name = "Copper Bar" };
        bar.Components.Add(copper, 5);
        await _blueprintDAO.SaveAsync(bar);

        // The link tables carry no DatasetId of their own, so this is what proves they are scoped
        // through their parent rather than read wholesale.
        _fixture.SelectDataset(_secondDatasetId);
        BlueprintModel sameName = new() { Name = "Copper Bar" };
        await _blueprintDAO.SaveAsync(sameName);

        BlueprintSummary summary = (await _blueprintDAO.GetSummariesAsync()).Should().ContainSingle().Subject;
        BlueprintModel loaded = (await _blueprintDAO.GetByIdAsync(summary.Id))!;
        loaded.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAllData_LeavesTheOtherDatasetIntact()
    {
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });

        _fixture.SelectDataset(_secondDatasetId);
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur" });

        await new DatabaseAdminDAO(_fixture.DatasetFactory).DeleteAllDataAsync();

        (await _componentDAO.GetAllAsync()).Should().BeEmpty();

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Name.Should().Be("Copper");
    }

    [Test]
    public async Task DeletingRecordsByIdOrAll_LeavesTheOtherDatasetIntact()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        ComponentModel copper = await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });

        _fixture.SelectDataset(_secondDatasetId);
        await _categoryDAO.SaveAsync(new CategoryModel { Name = "Weapons" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur" });
        await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Arrow" });

        // Ids from the other dataset are ignored rather than deleted.
        await _categoryDAO.DeleteAsync([ores.Id]);
        await _componentDAO.DeleteAsync([copper.Id]);
        await _blueprintDAO.DeleteAsync([bronze.Id]);

        (await _categoryDAO.CountAsync()).Should().Be(1);
        (await _componentDAO.CountAsync()).Should().Be(1);
        (await _blueprintDAO.CountAsync()).Should().Be(1);

        await _categoryDAO.DeleteAllAsync();
        await _componentDAO.DeleteAllAsync();
        await _blueprintDAO.DeleteAllAsync();

        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        (await _categoryDAO.GetAllAsync()).Should().ContainSingle().Which.Name.Should().Be("Ores");
        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Name.Should().Be("Copper");
        (await _blueprintDAO.GetSummariesAsync()).Should().ContainSingle().Which.Name.Should().Be("Bronze");
    }
}
