using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class RecipeDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private RecipeDAO _recipeDAO = null!;
    private IngredientDAO _ingredientDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _recipeDAO = new RecipeDAO(_fixture.Factory);
        _ingredientDAO = new IngredientDAO(_fixture.Factory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task SaveAsync_NewRecipeWithIngredientAndChildRecipe_PersistsTheFullGraph()
    {
        Ingredient wood = await _ingredientDAO.SaveAsync(new Ingredient { Name = "Wood", Cost = 1 });
        Recipe plank = await _recipeDAO.SaveAsync(new Recipe { Name = "Plank" });

        Recipe table = new() { Name = "Table", Value = 10 };
        table.Ingredients.Add(wood, 2);
        table.ChildRecipes.Add(plank, 4);

        Recipe saved = await _recipeDAO.SaveAsync(table);
        saved.Id.Should().BeGreaterThan(0);

        Recipe? reloaded = await _recipeDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Ingredients.IngredientList.Should().ContainSingle(i => i.Ingredient.Name == "Wood" && i.Quantity == 2);
        reloaded.ChildRecipes.RecipeList.Should().ContainSingle(r => r.Recipe.Name == "Plank" && r.Quantity == 4);
    }

    [Test]
    public async Task SaveAsync_UpdatingExistingIngredientQuantity_UpdatesInPlaceRatherThanDuplicating()
    {
        Ingredient wood = await _ingredientDAO.SaveAsync(new Ingredient { Name = "Wood", Cost = 1 });
        Recipe table = new() { Name = "Table" };
        table.Ingredients.Add(wood, 2);
        Recipe saved = await _recipeDAO.SaveAsync(table);

        Recipe reloaded = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        IngredientQuantity existingIngredient = reloaded.Ingredients.IngredientList.Single();
        existingIngredient.Quantity = 9;

        await _recipeDAO.SaveAsync(reloaded);

        Recipe reloadedAgain = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Ingredients.IngredientList.Should().ContainSingle();
        reloadedAgain.Ingredients.IngredientList[0].Quantity.Should().Be(9);
    }

    [Test]
    public async Task SaveAsync_RemovedIngredient_DeletesItsRowRatherThanLeavingItOrphaned()
    {
        Ingredient wood = await _ingredientDAO.SaveAsync(new Ingredient { Name = "Wood", Cost = 1 });
        Recipe table = new() { Name = "Table" };
        table.Ingredients.Add(wood, 2);
        Recipe saved = await _recipeDAO.SaveAsync(table);

        Recipe reloaded = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloaded.Ingredients.Remove(wood, reloaded.Ingredients.IngredientList[0].Quantity);

        await _recipeDAO.SaveAsync(reloaded);

        Recipe reloadedAgain = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Ingredients.IngredientList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_RemovesTheRecipe()
    {
        Recipe saved = await _recipeDAO.SaveAsync(new Recipe { Name = "Table" });

        await _recipeDAO.DeleteAsync(saved.Id);

        (await _recipeDAO.GetByIdAsync(saved.Id)).Should().BeNull();
    }
}
