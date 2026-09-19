using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// What <see cref="DatasetDAO.ImportAsNewAsync"/> and <see cref="DatasetDAO.MergeAsync"/> promise: every record lands
/// where the plan says, every link points at the row its target landed on, and a failure writes nothing.
/// </summary>
[TestFixture]
public class DatasetImportTests
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

    /// <summary>
    /// Fills the selected dataset with Valheim's bronze chain - Copper and Tin under Metals, Wood, Bronze from 2 Copper
    /// and 1 Tin, a Bronze Axe from 4 Wood and 8 Bronze, and a favorite holding five axes - and returns it as a snapshot.
    /// </summary>
    private async Task<DatasetSnapshot> GivenTheBronzeChainAsync()
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

        BlueprintModel bronze = new() { Name = "Bronze", Value = 6, Yield = 2, Category = metals };
        bronze.Components.Add(copper, 2);
        bronze.Components.Add(tin, 1);
        await _blueprintDAO.SaveAsync(bronze);

        BlueprintModel axe = new() { Name = "Bronze Axe", Value = 40 };
        axe.Components.Add(wood, 4);
        axe.ChildBlueprints.Add(bronze, 8);
        await _blueprintDAO.SaveAsync(axe);

        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Bronze Axe run" }, [new BlueprintQuantity(axe, 5)]);

        return await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);
    }

    /// <summary>A snapshot with every id replaced by the name it points at, so two datasets' records compare.</summary>
    private static object Shape(DatasetSnapshot snapshot)
    {
        return new
        {
            Categories = snapshot.Categories.Select(category => new { category.Name, category.Description }),
            Components = snapshot.Components.Select(component => new
            {
                component.Name,
                component.Description,
                component.Cost,
                component.ProductionTime,
                Category = Category(component.CategoryId)
            }),
            Blueprints = snapshot.Blueprints.Select(blueprint => new
            {
                blueprint.Name,
                blueprint.Description,
                blueprint.Value,
                blueprint.Yield,
                blueprint.ProductionTime,
                Category = Category(blueprint.CategoryId),
                Components = blueprint.Components.Select(link => (Component(link.TargetId), link.Quantity)),
                Blueprints = blueprint.Blueprints.Select(link => (Blueprint(link.TargetId), link.Quantity))
            }),
            Favorites = snapshot.Favorites.Select(favorite => new
            {
                favorite.Name,
                Blueprints = favorite.Blueprints.Select(link => (Blueprint(link.TargetId), link.Quantity))
            })
        };

        string Category(int? id) => snapshot.Categories.SingleOrDefault(category => category.Id == id)?.Name ?? "";
        string Component(int id) => snapshot.Components.Single(component => component.Id == id).Name;
        string Blueprint(int id) => snapshot.Blueprints.Single(blueprint => blueprint.Id == id).Name;
    }

    private static SnapshotComponent Named(DatasetSnapshot snapshot, string name) => snapshot.Components.Single(component => component.Name == name);

    private static SnapshotBlueprint BlueprintNamed(DatasetSnapshot snapshot, string name) => snapshot.Blueprints.Single(blueprint => blueprint.Name == name);

    private Task MergeAsync(DatasetSnapshot incoming, DatasetSnapshot current, params RecordKey[] replace) =>
        _datasetDAO.MergeAsync(
            SqliteTestFixture.DefaultDatasetId,
            new MergePlan(incoming, ImportConflictProcessor.Find(incoming, current), new HashSet<RecordKey>(replace)));

    [Test]
    public async Task ImportAsNewAsync_ReproducesEveryRecordAndLink()
    {
        DatasetSnapshot source = await GivenTheBronzeChainAsync();

        DatasetModel imported = await _datasetDAO.ImportAsNewAsync("Valheim - Friend's", source);

        imported.Name.Should().Be("Valheim - Friend's");
        Shape(await _datasetDAO.GetSnapshotAsync(imported.Id)).Should().BeEquivalentTo(Shape(source));
    }

    [TestCase(false, false, true, true)]
    [TestCase(true, false, false, true)]
    [TestCase(true, true, true, false)]
    public async Task ImportAsNewAsync_TakesTheFilesSettings(
        bool useYield, bool useCosts, bool useValues, bool useCraftTime)
    {
        Datasettings settings =
            new(UseYield: useYield, UseCosts: useCosts, UseValues: useValues, UseCraftTime: useCraftTime);
        DatasetSnapshot source = await GivenTheBronzeChainAsync() with { Settings = settings };

        DatasetModel imported = await _datasetDAO.ImportAsNewAsync("Valheim - Friend's", source);

        (await _datasetDAO.GetSnapshotAsync(imported.Id)).Settings.Should().Be(settings);
    }

    [Test]
    public async Task MergeAsync_KeepsTheDatasetsOwnSettings()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        DatasetSnapshot incoming = new("Friend's Valheim", [], [new SnapshotComponent(1, "Resin", "", 1, TimeSpan.Zero, null)],
            [], [], new Datasettings(UseYield: false, UseCosts: false, UseValues: false, UseCraftTime: false));

        await MergeAsync(incoming, current);

        (await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId)).Settings.Should().Be(Datasettings.Default);
    }

    [Test]
    public async Task ImportAsNewAsync_LeavesTheOtherDatasetsAlone()
    {
        DatasetSnapshot source = await GivenTheBronzeChainAsync();

        await _datasetDAO.ImportAsNewAsync("Valheim - Friend's", source);

        (await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId)).Should().BeEquivalentTo(source);
    }

    [Test]
    public async Task ImportAsNewAsync_AFailurePartway_AddsNoDataset()
    {
        DatasetSnapshot source = await GivenTheBronzeChainAsync();

        // A link to a component the snapshot doesn't hold fails once the dataset row has already been saved.
        DatasetSnapshot broken = source with
        {
            Blueprints = [source.Blueprints[0] with { Components = [new QuantityLink(999, 1)] }],
            Favorites = []
        };

        Func<Task> act = () => _datasetDAO.ImportAsNewAsync("Broken", broken);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        (await _datasetDAO.GetAllAsync()).Select(dataset => dataset.Name).Should().Equal("Default");
    }

    [Test]
    public async Task MergeAsync_KeepMine_LinksAddedBlueprintsToTheExistingRecords()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        SnapshotComponent copper = Named(current, "Copper");

        // A friend's file with a pricier Copper and a Copper Plate made from it.
        DatasetSnapshot incoming = new("Friend's Valheim", [],
            [new SnapshotComponent(1, "copper", "", 99, TimeSpan.Zero, null)],
            [new SnapshotBlueprint(1, "Copper Plate", "", 12, 1, TimeSpan.Zero, null, [new QuantityLink(1, 3)], [])],
            [], Datasettings.Default);

        await MergeAsync(incoming, current);

        DatasetSnapshot merged = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);

        merged.Components.Should().HaveCount(3);
        Named(merged, "Copper").Should().Be(copper);
        BlueprintNamed(merged, "Copper Plate").Components.Should().Equal(new QuantityLink(copper.Id, 3));
    }

    [Test]
    public async Task MergeAsync_ReplaceMine_OverwritesTheRowKeepingItsId()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        SnapshotBlueprint bronze = BlueprintNamed(current, "Bronze");
        int copperId = Named(current, "Copper").Id;

        // The file's Bronze is uncategorized, worth more, and made from 3 Copper alone.
        DatasetSnapshot incoming = new("Friend's Valheim", [],
            [new SnapshotComponent(1, "Copper", "", 2, TimeSpan.Zero, null)],
            [new SnapshotBlueprint(1, "Bronze", "Better alloy", 10, 1, TimeSpan.FromMinutes(2), null, [new QuantityLink(1, 3)], [])],
            [], Datasettings.Default);

        await MergeAsync(incoming, current, new RecordKey(RecordKind.Blueprint, 1));

        DatasetSnapshot merged = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);
        SnapshotBlueprint replaced = BlueprintNamed(merged, "Bronze");

        replaced.Should().BeEquivalentTo(new SnapshotBlueprint(
            bronze.Id, "Bronze", "Better alloy", 10, 1, TimeSpan.FromMinutes(2), null, [new QuantityLink(copperId, 3)], []));

        // Kept, not replaced: Copper wasn't in the replace set.
        Named(merged, "Copper").Cost.Should().Be(2);

        // The Axe was never in the file, and still nests the same Bronze row.
        BlueprintNamed(merged, "Bronze Axe").Blueprints.Should().Equal(new QuantityLink(bronze.Id, 8));
    }

    [Test]
    public async Task MergeAsync_ReplacingAComponent_ClearsACategoryTheFileDoesNotHave()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        DatasetSnapshot incoming = new("Friend's Valheim", [], [new SnapshotComponent(1, "Tin", "", 3, TimeSpan.Zero, null)], [], [], Datasettings.Default);

        await MergeAsync(incoming, current, new RecordKey(RecordKind.Component, 1));

        SnapshotComponent tin = Named(await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId), "Tin");
        (tin.Cost, tin.CategoryId).Should().Be((3.0, null));
    }

    [Test]
    public async Task MergeAsync_ChooseEach_ReplacesOnlyThePicks()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        DatasetSnapshot incoming = new("Friend's Valheim", [],
            [new SnapshotComponent(1, "Copper", "", 5, TimeSpan.Zero, null), new SnapshotComponent(2, "Tin", "", 7, TimeSpan.Zero, null)],
            [], [], Datasettings.Default);

        await MergeAsync(incoming, current, new RecordKey(RecordKind.Component, 1));

        DatasetSnapshot merged = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);
        (Named(merged, "Copper").Cost, Named(merged, "Tin").Cost).Should().Be((5.0, 1.0));
    }

    [Test]
    public async Task MergeAsync_ReplacingAFavorite_ReplacesWhatItHolds()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        int bronzeId = BlueprintNamed(current, "Bronze").Id;
        DatasetSnapshot incoming = new("Friend's Valheim", [], [],
            [new SnapshotBlueprint(1, "Bronze", "", 6, 2, TimeSpan.Zero, null, [], [])],
            [new SnapshotFavorite(1, "Bronze Axe run", [new QuantityLink(1, 30)])], Datasettings.Default);

        await MergeAsync(incoming, current, new RecordKey(RecordKind.Favorite, 1));

        DatasetSnapshot merged = await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId);
        merged.Favorites.Single().Blueprints.Should().Equal(new QuantityLink(bronzeId, 30));

        // The file's Bronze was kept rather than replaced, so mine still has its Copper and Tin.
        BlueprintNamed(merged, "Bronze").Components.Should().HaveCount(2);
    }

    [Test]
    public async Task MergeAsync_AFailurePartway_WritesNothing()
    {
        DatasetSnapshot current = await GivenTheBronzeChainAsync();
        DatasetSnapshot incoming = new("Friend's Valheim",
            [new SnapshotCategory(1, "Tools", "")],
            [new SnapshotComponent(1, "Copper", "", 99, TimeSpan.Zero, null)],
            [], [], Datasettings.Default);

        // A conflict naming a row that isn't there: the dataset changed between the conflict check and the merge.
        MergePlan plan = new(incoming, [new ImportConflict(RecordKind.Component, 1, 999, "Copper")],
            new HashSet<RecordKey> { new(RecordKind.Component, 1) });

        Func<Task> act = () => _datasetDAO.MergeAsync(SqliteTestFixture.DefaultDatasetId, plan);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        (await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId)).Should().BeEquivalentTo(current);
    }

    [Test]
    public async Task MergeAsync_IntoADatasetThatIsNotSelected_LeavesTheSelectedOneAlone()
    {
        DatasetSnapshot valheim = await GivenTheBronzeChainAsync();
        DatasetModel rust = await _datasetDAO.AddAsync("Rust");
        _fixture.SelectDataset(rust.Id);
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Sulfur", Cost = 3 });
        DatasetSnapshot rustBefore = await _datasetDAO.GetSnapshotAsync(rust.Id);

        DatasetSnapshot incoming = new("Friend's Valheim", [], [new SnapshotComponent(1, "Resin", "", 1, TimeSpan.Zero, null)], [], [], Datasettings.Default);
        await _datasetDAO.MergeAsync(SqliteTestFixture.DefaultDatasetId,
            new MergePlan(incoming, ImportConflictProcessor.Find(incoming, valheim), new HashSet<RecordKey>()));

        (await _datasetDAO.GetSnapshotAsync(rust.Id)).Should().BeEquivalentTo(rustBefore);
        (await _datasetDAO.GetSnapshotAsync(SqliteTestFixture.DefaultDatasetId)).Components
            .Select(component => component.Name).Should().Equal("Copper", "Tin", "Wood", "Resin");
    }
}
