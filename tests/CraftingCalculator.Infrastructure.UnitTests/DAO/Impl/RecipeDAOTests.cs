using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class RecipeDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private RecipeDAO _recipeDAO = null!;
    private ComponentDAO _componentDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _recipeDAO = new RecipeDAO(_fixture.Factory);
        _componentDAO = new ComponentDAO(_fixture.Factory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task SaveAsync_NewRecipeWithComponentAndChildRecipe_PersistsTheFullGraph()
    {
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Recipe plank = await _recipeDAO.SaveAsync(new Recipe { Name = "Plank" });

        Recipe table = new() { Name = "Table", Value = 10 };
        table.Components.Add(wood, 2);
        table.ChildRecipes.Add(plank, 4);

        Recipe saved = await _recipeDAO.SaveAsync(table);
        saved.Id.Should().BeGreaterThan(0);

        Recipe? reloaded = await _recipeDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Components.ComponentList.Should().ContainSingle(i => i.Component.Name == "Wood" && i.Quantity == 2);
        reloaded.ChildRecipes.RecipeList.Should().ContainSingle(r => r.Recipe.Name == "Plank" && r.Quantity == 4);
    }

    [Test]
    public async Task SaveAsync_UpdatingExistingComponentQuantity_UpdatesInPlaceRatherThanDuplicating()
    {
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Recipe table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        Recipe saved = await _recipeDAO.SaveAsync(table);

        Recipe reloaded = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        ComponentQuantity existingComponent = reloaded.Components.ComponentList.Single();
        existingComponent.Quantity = 9;

        await _recipeDAO.SaveAsync(reloaded);

        Recipe reloadedAgain = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().ContainSingle();
        reloadedAgain.Components.ComponentList[0].Quantity.Should().Be(9);
    }

    [Test]
    public async Task SaveAsync_RemovedComponent_DeletesItsRowRatherThanLeavingItOrphaned()
    {
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Recipe table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        Recipe saved = await _recipeDAO.SaveAsync(table);

        Recipe reloaded = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloaded.Components.Remove(wood, reloaded.Components.ComponentList[0].Quantity);

        await _recipeDAO.SaveAsync(reloaded);

        Recipe reloadedAgain = (await _recipeDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_RemovesTheRecipe()
    {
        Recipe saved = await _recipeDAO.SaveAsync(new Recipe { Name = "Table" });

        await _recipeDAO.DeleteAsync(saved.Id);

        (await _recipeDAO.GetByIdAsync(saved.Id)).Should().BeNull();
    }
}
