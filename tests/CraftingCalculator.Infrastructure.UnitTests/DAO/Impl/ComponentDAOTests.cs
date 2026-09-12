using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class ComponentDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private ComponentDAO _componentDAO = null!;
    private CategoryDAO _categoryDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _componentDAO = new ComponentDAO(_fixture.DatasetFactory);
        _categoryDAO = new CategoryDAO(_fixture.DatasetFactory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task SaveAsync_ComponentWithCategory_RoundTripsThatCategory()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });

        ComponentModel saved = await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Cost = 3, Category = ores });
        saved.Id.Should().BeGreaterThan(0);

        ComponentModel? reloaded = await _componentDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Category.Should().NotBeNull();
        reloaded.Category!.Id.Should().Be(ores.Id);
        reloaded.Category.Name.Should().Be("Ores");
    }

    [Test]
    public async Task SaveAsync_ClearingTheCategory_PersistsAsNoCategory()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        ComponentModel saved = await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Category = ores });

        saved.Category = null;
        await _componentDAO.SaveAsync(saved);

        ComponentModel? reloaded = await _componentDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Category.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_ReturnsEachComponentWithItsOwnCategory()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Category = ores });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood" });

        List<ComponentModel> all = await _componentDAO.GetAllAsync();

        all.Should().ContainSingle(component => component.Name == "Iron" && component.Category!.Name == "Ores");
        all.Should().ContainSingle(component => component.Name == "Wood" && component.Category == null);
    }
}
