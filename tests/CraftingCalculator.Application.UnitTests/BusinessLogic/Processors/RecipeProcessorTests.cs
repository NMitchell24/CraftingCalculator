using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class RecipeProcessorTests
{
    private static Ingredient NewIngredient(string name) => new Ingredient { Id = 1, Name = name };

    private static Recipe NewRecipe(string name) => new Recipe { Id = 1, Name = name };

    [Test]
    public void Flatten_SingleLevel_ReturnsOwnIngredients()
    {
        Recipe recipe = NewRecipe("Widget");
        recipe.Ingredients.Add(NewIngredient("Screw"), 2);
        recipe.Ingredients.Add(NewIngredient("Plate"), 1);

        IngredientMap result = RecipeProcessor.Flatten(recipe);

        result.IngredientList.Should().HaveCount(2);
        result.IngredientList.Single(i => i.Name == "Screw").Quantity.Should().Be(2);
        result.IngredientList.Single(i => i.Name == "Plate").Quantity.Should().Be(1);
    }

    [Test]
    public void Flatten_TwoLevels_ScalesChildIngredientsByChildQuantity()
    {
        Recipe child = NewRecipe("Bracket");
        child.Ingredients.Add(NewIngredient("Screw"), 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        IngredientMap result = RecipeProcessor.Flatten(parent);

        result.IngredientList.Should().ContainSingle();
        result.IngredientList[0].Name.Should().Be("Screw");
        result.IngredientList[0].Quantity.Should().Be(6); // 3 per Bracket x2 Brackets
    }

    [Test]
    public void Flatten_ThreeLevels_MultipliesQuantityAcrossEveryLevel()
    {
        Recipe grandchild = NewRecipe("Rivet Set");
        grandchild.Ingredients.Add(NewIngredient("Rivet"), 1);

        Recipe child = NewRecipe("Bracket");
        child.ChildRecipes.Add(grandchild, 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        IngredientMap result = RecipeProcessor.Flatten(parent);

        result.IngredientList.Should().ContainSingle();
        result.IngredientList[0].Name.Should().Be("Rivet");
        result.IngredientList[0].Quantity.Should().Be(6); // 1 x3 Rivet Sets x2 Brackets
    }

    [Test]
    public void Flatten_Diamond_CombinesSharedIngredientFromBothBranches()
    {
        Recipe left = NewRecipe("Left Arm");
        left.Ingredients.Add(NewIngredient("Bolt"), 1);

        Recipe right = NewRecipe("Right Arm");
        right.Ingredients.Add(NewIngredient("Bolt"), 1);

        Recipe parent = NewRecipe("Chassis");
        parent.ChildRecipes.Add(left, 1);
        parent.ChildRecipes.Add(right, 1);

        IngredientMap result = RecipeProcessor.Flatten(parent);

        result.IngredientList.Should().ContainSingle();
        result.IngredientList[0].Name.Should().Be("Bolt");
        result.IngredientList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void Flatten_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Recipe recipe = NewRecipe("Self Referencing");
        recipe.ChildRecipes.Add(recipe, 1);

        Action act = () => RecipeProcessor.Flatten(recipe);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void BuildNode_ScalesNodeNamesByQuantity()
    {
        Recipe child = NewRecipe("Bracket");
        child.Ingredients.Add(NewIngredient("Screw"), 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        RecipeNode tree = RecipeProcessor.BuildNode(parent, 1);

        tree.Name.Should().Be("Frame x1");
        RecipeNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Name.Should().Be("Bracket x2");
        childNode.IsIngredient.Should().BeFalse();
        RecipeNode ingredientNode = childNode.Children.Should().ContainSingle().Subject;
        ingredientNode.Name.Should().Be("Screw x6");
        ingredientNode.IsIngredient.Should().BeTrue();
    }

    [Test]
    public void BuildNode_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Recipe recipe = NewRecipe("Self Referencing");
        recipe.ChildRecipes.Add(recipe, 1);

        Action act = () => RecipeProcessor.BuildNode(recipe, 1);

        act.Should().Throw<InvalidOperationException>();
    }
}
