using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class BlueprintFavoritesDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private BlueprintFavoritesDAO _favoritesDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _blueprintDAO = new BlueprintDAO(_fixture.Factory);
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.Factory, _blueprintDAO);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task GetAllAsync_ReturnsEachFavoritesSavedBlueprintCount()
    {
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });
        BlueprintModel table = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Table" });

        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Two" },
            [new BlueprintQuantity(plank, 3, 0), new BlueprintQuantity(table, 1, 0)]);
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "One" }, [new BlueprintQuantity(plank, 5, 0)]);
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Empty" }, []);

        List<BlueprintFavorite> favorites = await _favoritesDAO.GetAllAsync();

        favorites.Select(f => f.Name).Should().Equal("Empty", "One", "Two");
        favorites.Select(f => f.BlueprintCount).Should().Equal(0, 1, 2);
    }

    [Test]
    public async Task RenameAsync_ChangesTheNameAndLeavesTheSavedQuantitiesIntact()
    {
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });
        BlueprintFavorite saved = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Old" },
            [new BlueprintQuantity(plank, 7, 0)]);

        await _favoritesDAO.RenameAsync(saved.Id, "New");

        (await _favoritesDAO.GetByNameAsync("Old")).Should().BeNull();
        (await _favoritesDAO.GetByNameAsync("New")).Should().NotBeNull();

        List<BlueprintQuantity> quantities = await _favoritesDAO.GetBlueprintQuantitiesAsync(saved.Id);
        quantities.Should().ContainSingle(q => q.Blueprint.Name == "Plank" && q.Quantity == 7);
    }
}
