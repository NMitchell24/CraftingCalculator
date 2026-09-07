using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class RecipePartProcessorTests
{
    private static Component NewComponent(int id, string name) => new Component { Id = id, Name = name };

    private static Recipe NewRecipe(int id, string name) => new Recipe { Id = id, Name = name };

    [Test]
    public void GetComponents_ReturnsComponentsBeforeChildRecipes_EachOrderedByName()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Components.Add(NewComponent(2, "Screw"), 4);
        recipe.Components.Add(NewComponent(3, "Bolt"), 1);
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2);

        List<IBaseQuantityRecord> components = RecipePartProcessor.GetParts(recipe);

        components.Select(c => c.Name).Should().Equal("Bolt", "Screw", "Bracket");
    }

    [Test]
    public void Add_NewComponent_AddsItToTheRecipe()
    {
        Recipe recipe = NewRecipe(1, "Frame");

        RecipePartProcessor.Add(recipe, NewComponent(2, "Screw"), 3);

        recipe.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(3);
    }

    [Test]
    public void Add_ComponentAlreadyOnTheRecipe_RaisesItsQuantity()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Components.Add(NewComponent(2, "Screw"), 3);

        RecipePartProcessor.Add(recipe, NewComponent(2, "Screw"), 2);

        recipe.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(5);
    }

    [Test]
    public void Add_ChildRecipe_AddsItToTheRecipe()
    {
        Recipe recipe = NewRecipe(1, "Frame");

        RecipePartProcessor.Add(recipe, NewRecipe(4, "Bracket"), 2);

        recipe.ChildRecipes.RecipeList.Should().ContainSingle()
            .Which.Quantity.Should().Be(2);
    }

    [Test]
    public void SetQuantity_PositiveQuantity_ReplacesTheQuantityRatherThanAddingToIt()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Components.Add(NewComponent(2, "Screw"), 3);

        RecipePartProcessor.SetQuantity(recipe, recipe.Components.ComponentList[0], 7);

        recipe.Components.ComponentList[0].Quantity.Should().Be(7);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheComponentAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Components.Add(NewComponent(2, "Screw"), 3, id: 11);

        RecipePartProcessor.SetQuantity(recipe, recipe.Components.ComponentList[0], 0);

        recipe.Components.ComponentList.Should().BeEmpty();
        recipe.Components.RemovedComponents.Should().ContainSingle().Which.Id.Should().Be(11);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheChildRecipeAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2, id: 12);

        RecipePartProcessor.SetQuantity(recipe, recipe.ChildRecipes.RecipeList[0], 0);

        recipe.ChildRecipes.RecipeList.Should().BeEmpty();
        recipe.ChildRecipes.RemovedRecipes.Should().ContainSingle().Which.Id.Should().Be(12);
    }

    [Test]
    public void Remove_ChildRecipe_RemovesItAndRecordsItForTheDAO()
    {
        Recipe recipe = NewRecipe(1, "Frame");
        recipe.Components.Add(NewComponent(2, "Screw"), 3, id: 11);
        recipe.ChildRecipes.Add(NewRecipe(4, "Bracket"), 2, id: 12);

        RecipePartProcessor.Remove(recipe, recipe.ChildRecipes.RecipeList[0]);

        recipe.ChildRecipes.RecipeList.Should().BeEmpty();
        recipe.ChildRecipes.RemovedRecipes.Should().ContainSingle().Which.Id.Should().Be(12);
        recipe.Components.ComponentList.Should().ContainSingle();
    }
}
