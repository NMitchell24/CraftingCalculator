using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// What <see cref="DatasetDAO.CopyAsync"/> promises: the copy holds its own records, and every link
/// between them points inside the copy rather than back at the dataset it came from. Nothing in the
/// entity model enforces that - the foreign keys are as happy pointing across datasets as within one -
/// so this is where the remapping is pinned down.
/// </summary>
[TestFixture]
public class DatasetCopyTests
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
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.DatasetFactory, _blueprintDAO);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    /// <summary>
    /// Fills the selected dataset with Valheim's bronze chain - a category, three components, a blueprint
    /// built from components, a blueprint built from that blueprint, and a favorite holding it - so one
    /// source covers every link the copy has to remap.
    /// </summary>
    private async Task GivenTheBronzeChainAsync()
    {
        CategoryModel metals = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Metals" });

        ComponentModel copper = await _componentDAO.SaveAsync(
            new ComponentModel { Name = "Copper", Cost = 2, Category = metals });
        ComponentModel tin = await _componentDAO.SaveAsync(
            new ComponentModel { Name = "Tin", Cost = 1, Category = metals });
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 0.5 });

        BlueprintModel bronze = new() { Name = "Bronze", Value = 6, Category = metals };
        bronze.Components.Add(copper, 2);
        bronze.Components.Add(tin, 1);
        await _blueprintDAO.SaveAsync(bronze);

        BlueprintModel axe = new() { Name = "Bronze Axe", Value = 40 };
        axe.Components.Add(wood, 4);
        axe.ChildBlueprints.Add(bronze, 8);
        await _blueprintDAO.SaveAsync(axe);

        await _favoritesDAO.SaveAsync(
            new BlueprintFavorite { Name = "Starter kit" }, [new BlueprintQuantity(axe, 2, 0)]);
    }

    [Test]
    public async Task CopyAsync_ReproducesEveryRecord()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);

        (await _categoryDAO.GetAllAsync()).Select(category => category.Name).Should().Equal("Metals");
        (await _componentDAO.GetAllAsync()).Select(component => component.Name).Should().Equal("Copper", "Tin", "Wood");
        (await _blueprintDAO.GetAllAsync()).Select(blueprint => blueprint.Name).Should().Equal("Bronze", "Bronze Axe");
        (await _favoritesDAO.GetAllAsync()).Select(favorite => favorite.Name).Should().Equal("Starter kit");
    }

    [Test]
    public async Task CopyAsync_CopiesEveryFieldOfARecord()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        ComponentModel copper = (await _componentDAO.GetAllAsync()).First(component => component.Name == "Copper");

        copper.Cost.Should().Be(2);

        // The category is the copy's own, not the one the source component was filed under.
        copper.Category!.Name.Should().Be("Metals");
        copper.Category.Id.Should().Be((await _categoryDAO.GetAllAsync()).Single().Id);
    }

    [Test]
    public async Task CopyAsync_PointsAComponentLinkAtTheCopiedComponent()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        List<int> componentIds = [.. (await _componentDAO.GetAllAsync()).Select(component => component.Id)];
        BlueprintModel bronze = (await _blueprintDAO.GetAllAsync()).First(blueprint => blueprint.Name == "Bronze");

        bronze.Components.ComponentList.Select(quantity => (quantity.Name, quantity.Quantity))
            .Should().Equal(("Copper", 2L), ("Tin", 1L));

        // A link still pointing at the source dataset's Copper would drop out of the model entirely, since
        // BlueprintDAO reads components through the dataset filter - this says which Copper it found.
        bronze.Components.ComponentList.Select(quantity => quantity.Component.Id).Should().BeSubsetOf(componentIds);
    }

    [Test]
    public async Task CopyAsync_PointsAChildBlueprintLinkAtTheCopiedBlueprint()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        List<BlueprintModel> blueprints = await _blueprintDAO.GetAllAsync();
        BlueprintModel axe = blueprints.First(blueprint => blueprint.Name == "Bronze Axe");
        BlueprintModel bronze = blueprints.First(blueprint => blueprint.Name == "Bronze");

        BlueprintQuantity child = axe.ChildBlueprints.BlueprintList.Should().ContainSingle().Subject;

        child.Quantity.Should().Be(8);
        child.Blueprint.Id.Should().Be(bronze.Id);

        // The nested blueprint arrives with its own components, which is the whole tree having been
        // remapped rather than just its top level.
        child.Blueprint.Components.ComponentList.Select(quantity => quantity.Name).Should().Equal("Copper", "Tin");
    }

    [Test]
    public async Task CopyAsync_PointsAFavoriteAtTheCopiedBlueprints()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        BlueprintFavorite favorite = (await _favoritesDAO.GetAllAsync()).Single();
        List<BlueprintQuantity> quantities = await _favoritesDAO.GetBlueprintQuantitiesAsync(favorite.Id);

        BlueprintQuantity saved = quantities.Should().ContainSingle().Subject;

        saved.Quantity.Should().Be(2);
        saved.Blueprint.Id.Should().Be(
            (await _blueprintDAO.GetAllAsync()).First(blueprint => blueprint.Name == "Bronze Axe").Id);
    }

    [Test]
    public async Task CopyAsync_LeavesTheSourceDatasetAloneWhenTheCopyIsEdited()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        ComponentModel copper = (await _componentDAO.GetAllAsync()).First(component => component.Name == "Copper");
        copper.Cost = 99;
        await _componentDAO.SaveAsync(copper);
        await _componentDAO.DeleteAsync((await _componentDAO.GetAllAsync()).First(component => component.Name == "Wood").Id);

        // The reason to copy a dataset rather than edit the one you trust: the modded prices and the
        // records they were derived from are separate rows from here on.
        _fixture.SelectDataset(SqliteTestFixture.DefaultDatasetId);
        List<ComponentModel> original = await _componentDAO.GetAllAsync();

        original.Select(component => component.Name).Should().Equal("Copper", "Tin", "Wood");
        original.First(component => component.Name == "Copper").Cost.Should().Be(2);
    }

    [Test]
    public async Task CopyAsync_CopiesTheNamedDatasetRatherThanTheSelectedOne()
    {
        await GivenTheBronzeChainAsync();

        DatasetModel rust = await _datasetDAO.AddAsync("Rust");
        _fixture.SelectDataset(rust.Id);
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur", Cost = 3 });

        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        // Nothing about the copy reads the selection: the source is the id it was handed, and the rows it
        // finds are that dataset's.
        _fixture.SelectDataset(copy.Id);
        (await _componentDAO.GetAllAsync()).Select(component => component.Name).Should().Equal("Copper", "Tin", "Wood");
    }

    [Test]
    public async Task CopyAsync_AnEmptyDataset_AddsAnEmptyDataset()
    {
        DatasetModel copy = await _datasetDAO.CopyAsync(SqliteTestFixture.DefaultDatasetId, "Valheim - Modded");

        copy.Id.Should().NotBe(SqliteTestFixture.DefaultDatasetId);
        copy.Name.Should().Be("Valheim - Modded");

        _fixture.SelectDataset(copy.Id);
        (await _componentDAO.GetAllAsync()).Should().BeEmpty();
    }
}
