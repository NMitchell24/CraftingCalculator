using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class RecipeComponentProcessorTests
{
    private static Ingredient NewIngredient(int id, string name) => new Ingredient { Id = id, Name = name };

    private static Recipe NewRecipe(int id, string name) => new Recipe { Id = id, Name = name };

    [Test]
    public void GetComponents_ReturnsIngredientsBeforeChildRecipes_EachOrderedByName()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Ingredients.Add(NewIngredient(2, "Screw"), 4);
        recipe.Ingredients.Add(NewIngredient(3, "Bolt"), 1);
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2);

        List<IBaseQuantityRecord> components = RecipeComponentProcessor.GetComponents(recipe);

        components.Select(c => c.Name).Should().Equal("Bolt", "Screw", "Bracket");
    }

    [Test]
    public void Add_NewIngredient_AddsItToTheRecipe()
    {
        Recipe recipe = NewRecipe(1, "Frame");

        RecipeComponentProcessor.Add(recipe, NewIngredient(2, "Screw"), 3);

        recipe.Ingredients.IngredientList.Should().ContainSingle()
            .Which.Quantity.Should().Be(3);
    }

    [Test]
    public void Add_IngredientAlreadyOnTheRecipe_RaisesItsQuantity()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Ingredients.Add(NewIngredient(2, "Screw"), 3);

        RecipeComponentProcessor.Add(recipe, NewIngredient(2, "Screw"), 2);

        recipe.Ingredients.IngredientList.Should().ContainSingle()
            .Which.Quantity.Should().Be(5);
    }

    [Test]
    public void Add_ChildRecipe_AddsItToTheRecipe()
    {
        Recipe recipe = NewRecipe(1, "Frame");

        RecipeComponentProcessor.Add(recipe, NewRecipe(4, "Bracket"), 2);

        recipe.ChildRecipes.RecipeList.Should().ContainSingle()
            .Which.Quantity.Should().Be(2);
    }

    [Test]
    public void SetQuantity_PositiveQuantity_ReplacesTheQuantityRatherThanAddingToIt()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Ingredients.Add(NewIngredient(2, "Screw"), 3);

        RecipeComponentProcessor.SetQuantity(recipe, recipe.Ingredients.IngredientList[0], 7);

        recipe.Ingredients.IngredientList[0].Quantity.Should().Be(7);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheIngredientAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Ingredients.Add(NewIngredient(2, "Screw"), 3, id: 11);

        RecipeComponentProcessor.SetQuantity(recipe, recipe.Ingredients.IngredientList[0], 0);

        recipe.Ingredients.IngredientList.Should().BeEmpty();
        recipe.Ingredients.RemovedIngredients.Should().ContainSingle().Which.Id.Should().Be(11);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheChildRecipeAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2, id: 12);

        RecipeComponentProcessor.SetQuantity(recipe, recipe.ChildRecipes.RecipeList[0], 0);

        recipe.ChildRecipes.RecipeList.Should().BeEmpty();
        recipe.ChildRecipes.RemovedRecipes.Should().ContainSingle().Which.Id.Should().Be(12);
    }

    [Test]
    public void Remove_ChildRecipe_RemovesItAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Ingredients.Add(NewIngredient(2, "Screw"), 3, id: 11);
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2, id: 12);

        RecipeComponentProcessor.Remove(recipe, recipe.ChildRecipes.RecipeList[0]);

        recipe.ChildRecipes.RecipeList.Should().BeEmpty();
        recipe.ChildRecipes.RemovedRecipes.Should().ContainSingle().Which.Id.Should().Be(12);
        recipe.Ingredients.IngredientList.Should().ContainSingle();
    }
}
