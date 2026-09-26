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

    [Test]
    public async Task DeleteAsync_RemovesOnlyTheListedComponents()
    {
        ComponentModel copper = await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        ComponentModel tin = await _componentDAO.SaveAsync(new ComponentModel { Name = "Tin" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood" });

        await _componentDAO.DeleteAsync([copper.Id, tin.Id]);

        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Name.Should().Be("Wood");
    }

    [Test]
    public async Task DeleteAllAsync_RemovesEveryComponent()
    {
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Copper" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Tin" });

        await _componentDAO.DeleteAllAsync();

        (await _componentDAO.CountAsync()).Should().Be(0);
    }

    [Test]
    public async Task DeletingItsCategory_LeavesTheComponentWithNoCategory()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        CategoryModel wood = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Wood" });
        CategoryModel tools = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Tools" });
        ComponentModel iron = await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Category = ores });

        await _categoryDAO.DeleteAsync([ores.Id, wood.Id]);

        (await _categoryDAO.GetAllAsync()).Should().ContainSingle().Which.Id.Should().Be(tools.Id);
        (await _componentDAO.GetByIdAsync(iron.Id))!.Category.Should().BeNull();
    }

    [Test]
    public async Task DeletingEveryCategory_KeepsTheComponents()
    {
        CategoryModel ores = await _categoryDAO.SaveAsync(new CategoryModel { Name = "Ores" });
        await _componentDAO.SaveAsync(new ComponentModel { Name = "Iron", Category = ores });

        await _categoryDAO.DeleteAllAsync();

        (await _categoryDAO.CountAsync()).Should().Be(0);
        (await _componentDAO.GetAllAsync()).Should().ContainSingle().Which.Category.Should().BeNull();
    }
}
