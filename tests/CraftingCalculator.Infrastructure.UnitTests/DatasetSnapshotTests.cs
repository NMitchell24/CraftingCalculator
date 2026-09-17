using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// What <see cref="DatasetDAO.GetSnapshotAsync"/> promises: every field and every link of one dataset, and
/// nothing from any other.
/// </summary>
[TestFixture]
public class DatasetSnapshotTests
{
    private SqliteTestFixture _fixture = null!;
    private DatasetDAO _datasetDAO = null!;
    private CategoryDAO _categoryDAO = null!;
    private ComponentDAO _componentDAO = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private BlueprintFavoritesDAO _favoritesDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _datasetDAO = new DatasetDAO(_fixture.RawFactory);
        _categoryDAO = new CategoryDAO(_fixture.DatasetFactory);
        _componentDAO = new ComponentDAO(_fixture.DatasetFactory);
        _blueprintDAO = new BlueprintDAO(_fixture.DatasetFactory);
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.DatasetFactory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    /// <summary>Fills the selected dataset with Valheim's bronze chain and returns the records it saved.</summary>
    private async Task<(CategoryModel Metals, ComponentModel Copper, ComponentModel Tin, ComponentModel Wood,
        BlueprintModel Bronze, BlueprintModel Axe)> GivenTheBronzeChainAsync()
    {
        CategoryModel metals = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Metals", Description = "Smelted" });

        ComponentModel copper = await _componentDAO.SaveAsync(new ComponentModel
        {
            Name = "Copper",
            Description = "Ore",
            Cost = 2,
            ProductionTime = TimeSpan.FromSeconds(30),
            Category = metals
        });
        ComponentModel tin = await _componentDAO.SaveAsync(new ComponentModel { Name = "Tin", Cost = 1, Category = metals });
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 0.5 });

        BlueprintModel bronze = new()
        {
            Name = "Bronze",
            Description = "Alloy",
            Value = 6,
            Yield = 2,
            ProductionTime = TimeSpan.FromMinutes(1),
            Category = metals
        };
        bronze.Components.Add(copper, 2);
        bronze.Components.Add(tin, 1);
        await _blueprintDAO.SaveAsync(bronze);

        BlueprintModel axe = new() { Name = "Bronze Axe", Value = 40 };
        axe.Components.Add(wood, 4);
        axe.ChildBlueprints.Add(bronze, 8);
        await _blueprintDAO.SaveAsync(axe);

        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Bronze Axe run" }, [new BlueprintQuantity(axe, 5)]);

        return (metals, copper, tin, wood, bronze, axe);
    }

    [Test]
    public async Task GetSnapshotAsync_ReadsTheDatasetName()
    {
        DatasetSnapshot snapshot = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);

        snapshot.DatasetName.Should().Be("Default");
    }

    [Test]
    public async Task GetSnapshotAsync_ReadsEveryFieldOfEveryRecord()
    {
        (CategoryModel metals, ComponentModel copper, _, ComponentModel wood, BlueprintModel bronze, BlueprintModel axe) =
            await GivenTheBronzeChainAsync();

        DatasetSnapshot snapshot = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);

        snapshot.Categories.Should().Equal(new SnapshotCategory(metals.Id, "Metals", "Smelted"));
        snapshot.Components[0].Should().Be(
            new SnapshotComponent(copper.Id, "Copper", "Ore", 2, TimeSpan.FromSeconds(30), metals.Id));
        snapshot.Components[^1].CategoryId.Should().BeNull();
        snapshot.Components[^1].Id.Should().Be(wood.Id);

        SnapshotBlueprint savedBronze = snapshot.Blueprints.Single(blueprint => blueprint.Id == bronze.Id);
        (savedBronze.Name, savedBronze.Description, savedBronze.Value, savedBronze.Yield, savedBronze.ProductionTime, savedBronze.CategoryId)
            .Should().Be(("Bronze", "Alloy", 6.0, 2L, TimeSpan.FromMinutes(1), metals.Id));

        snapshot.Blueprints.Single(blueprint => blueprint.Id == axe.Id).CategoryId.Should().BeNull();
        snapshot.Favorites.Single().Name.Should().Be("Bronze Axe run");
    }

    [Test]
    public async Task GetSnapshotAsync_ReadsEveryLink()
    {
        (_, ComponentModel copper, ComponentModel tin, ComponentModel wood, BlueprintModel bronze, BlueprintModel axe) =
            await GivenTheBronzeChainAsync();

        DatasetSnapshot snapshot = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);

        SnapshotBlueprint savedBronze = snapshot.Blueprints.Single(blueprint => blueprint.Id == bronze.Id);
        SnapshotBlueprint savedAxe = snapshot.Blueprints.Single(blueprint => blueprint.Id == axe.Id);

        savedBronze.Components.Should().Equal(new QuantityLink(copper.Id, 2), new QuantityLink(tin.Id, 1));
        savedBronze.Blueprints.Should().BeEmpty();
        savedAxe.Components.Should().Equal(new QuantityLink(wood.Id, 4));
        savedAxe.Blueprints.Should().Equal(new QuantityLink(bronze.Id, 8));
        snapshot.Favorites.Single().Blueprints.Should().Equal(new QuantityLink(axe.Id, 5));
    }

    [Test]
    public async Task GetSnapshotAsync_LeavesOutAnotherDatasetsRecords()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel rust = await _datasetDAO.AddAsync("Rust");
        _fixture.SelectDataset(rust.Id);
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur", Cost = 3 });

        // Read with Rust selected: the snapshot follows the id it was given, not the selection.
        DatasetSnapshot valheim = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);
        DatasetSnapshot rustSnapshot = await _datasetDAO.GetSnapshotAsync(rust.Id);

        valheim.Components.Select(component => component.Name).Should().Equal("Copper", "Tin", "Wood");
        rustSnapshot.Components.Select(component => component.Name).Should().Equal("Sulfur");
        rustSnapshot.Blueprints.Should().BeEmpty();
        rustSnapshot.Favorites.Should().BeEmpty();
        rustSnapshot.DatasetName.Should().Be("Rust");
    }
}
