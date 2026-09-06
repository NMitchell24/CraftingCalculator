using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class RecipeFavoritesDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private RecipeDAO _recipeDAO = null!;
    private RecipeFavoritesDAO _favoritesDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _recipeDAO = new RecipeDAO(_fixture.Factory);
        _favoritesDAO = new RecipeFavoritesDAO(_fixture.Factory, _recipeDAO);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task GetAllAsync_ReturnsEachFavoritesSavedRecipeCount()
    {
        Recipe plank = await _recipeDAO.SaveAsync(new Recipe { Name = "Plank" });
        Recipe table = await _recipeDAO.SaveAsync(new Recipe { Name = "Table" });

        await _favoritesDAO.SaveAsync(new RecipeFavorite { Name = "Two" },
            [new RecipeQuantity(plank, 3, 0), new RecipeQuantity(table, 1, 0)]);
        await _favoritesDAO.SaveAsync(new RecipeFavorite { Name = "One" }, [new RecipeQuantity(plank, 5, 0)]);
        await _favoritesDAO.SaveAsync(new RecipeFavorite { Name = "Empty" }, []);

        List<RecipeFavorite> favorites = await _favoritesDAO.GetAllAsync();

        favorites.Select(f => f.Name).Should().Equal("Empty", "One", "Two");
        favorites.Select(f => f.RecipeCount).Should().Equal(0, 1, 2);
    }

    [Test]
    public async Task RenameAsync_ChangesTheNameAndLeavesTheSavedQuantitiesIntact()
    {
        Recipe plank = await _recipeDAO.SaveAsync(new Recipe { Name = "Plank" });
        RecipeFavorite saved = await _favoritesDAO.SaveAsync(new RecipeFavorite { Name = "Old" },
            [new RecipeQuantity(plank, 7, 0)]);

        await _favoritesDAO.RenameAsync(saved.Id, "New");

        (await _favoritesDAO.GetByNameAsync("Old")).Should().BeNull();
        (await _favoritesDAO.GetByNameAsync("New")).Should().NotBeNull();

        List<RecipeQuantity> quantities = await _favoritesDAO.GetRecipeQuantitiesAsync(saved.Id);
        quantities.Should().ContainSingle(q => q.Recipe.Name == "Plank" && q.Quantity == 7);
    }
}
