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
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.DatasetFactory, _blueprintDAO);

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
        (await _blueprintDAO.GetAllAsync()).Should().BeEmpty();
        (await _favoritesDAO.GetAllAsync()).Should().BeEmpty();
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

        BlueprintModel loaded = (await _blueprintDAO.GetAllAsync()).Should().ContainSingle().Subject;
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
}
