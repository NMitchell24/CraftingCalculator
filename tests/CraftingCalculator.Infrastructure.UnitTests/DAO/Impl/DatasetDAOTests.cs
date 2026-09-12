using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class DatasetDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private DatasetDAO _datasetDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();

        // The raw factory, matching how DatasetDAO is registered: the datasets themselves are what the
        // scoping is built on, so reading them cannot depend on a dataset being selected.
        _datasetDAO = new DatasetDAO(_fixture.RawFactory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task GetAllAsync_ReturnsTheMigrationSeededDefaultOrderedByName()
    {
        await _datasetDAO.AddAsync("Rust");
        await _datasetDAO.AddAsync("Ark");

        List<DatasetModel> datasets = await _datasetDAO.GetAllAsync();

        datasets.Select(dataset => dataset.Name).Should().Equal("Ark", "Default", "Rust");
    }

    [Test]
    public async Task AddAsync_AssignsAnId()
    {
        DatasetModel saved = await _datasetDAO.AddAsync("Valheim");

        saved.Id.Should().BeGreaterThan(0);
        (await _datasetDAO.GetByIdAsync(saved.Id))!.Name.Should().Be("Valheim");
    }

    [Test]
    public async Task GetByNameAsync_MatchesIgnoringCase()
    {
        await _datasetDAO.AddAsync("Valheim");

        (await _datasetDAO.GetByNameAsync("VALHEIM", exceptId: 0)).Should().NotBeNull();
        (await _datasetDAO.GetByNameAsync("valheim", exceptId: 0)).Should().NotBeNull();
        (await _datasetDAO.GetByNameAsync("Rust", exceptId: 0)).Should().BeNull();
    }

    [Test]
    public async Task GetByNameAsync_IgnoresTheDatasetBeingRenamed()
    {
        DatasetModel valheim = await _datasetDAO.AddAsync("Valheim");

        // Renaming a dataset to the name it already has is not a collision with itself.
        (await _datasetDAO.GetByNameAsync("Valheim", exceptId: valheim.Id)).Should().BeNull();
        (await _datasetDAO.GetByNameAsync("Valheim", exceptId: 0)).Should().NotBeNull();
    }

    [Test]
    public async Task GetByNameAsync_TreatsWildcardCharactersLiterally()
    {
        await _datasetDAO.AddAsync("Valheim");

        // Would match every dataset if the comparison were a LIKE with an unescaped pattern.
        (await _datasetDAO.GetByNameAsync("%", exceptId: 0)).Should().BeNull();
        (await _datasetDAO.GetByNameAsync("_______", exceptId: 0)).Should().BeNull();
    }

    [Test]
    public async Task RenameAsync_ChangesTheName()
    {
        DatasetModel valheim = await _datasetDAO.AddAsync("Valheim");

        await _datasetDAO.RenameAsync(valheim.Id, "Valheim - Modded");

        (await _datasetDAO.GetByIdAsync(valheim.Id))!.Name.Should().Be("Valheim - Modded");
    }

    [Test]
    public async Task CountAsync_CountsEveryDataset()
    {
        (await _datasetDAO.CountAsync()).Should().Be(1);

        await _datasetDAO.AddAsync("Rust");

        (await _datasetDAO.CountAsync()).Should().Be(2);
    }

    [Test]
    public async Task DeleteAsync_RemovesTheDataset()
    {
        DatasetModel rust = await _datasetDAO.AddAsync("Rust");

        await _datasetDAO.DeleteAsync(rust.Id);

        (await _datasetDAO.GetByIdAsync(rust.Id)).Should().BeNull();
    }
}
