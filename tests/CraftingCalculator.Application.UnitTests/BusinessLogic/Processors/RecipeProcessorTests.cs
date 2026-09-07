using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class RecipeProcessorTests
{
    private static Component NewComponent(string name) => new Component { Id = 1, Name = name };

    private static Recipe NewRecipe(string name) => new Recipe { Id = 1, Name = name };

    [Test]
    public void Flatten_SingleLevel_ReturnsOwnComponents()
    {
        Recipe recipe = NewRecipe("Widget");
        recipe.Components.Add(NewComponent("Screw"), 2);
        recipe.Components.Add(NewComponent("Plate"), 1);

        ComponentMap result = RecipeProcessor.Flatten(recipe);

        result.ComponentList.Should().HaveCount(2);
        result.ComponentList.Single(i => i.Name == "Screw").Quantity.Should().Be(2);
        result.ComponentList.Single(i => i.Name == "Plate").Quantity.Should().Be(1);
    }

    [Test]
    public void Flatten_TwoLevels_ScalesChildComponentsByChildQuantity()
    {
        Recipe child = NewRecipe("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        ComponentMap result = RecipeProcessor.Flatten(parent);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Screw");
        result.ComponentList[0].Quantity.Should().Be(6); // 3 per Bracket x2 Brackets
    }

    [Test]
    public void Flatten_ThreeLevels_MultipliesQuantityAcrossEveryLevel()
    {
        Recipe grandchild = NewRecipe("Rivet Set");
        grandchild.Components.Add(NewComponent("Rivet"), 1);

        Recipe child = NewRecipe("Bracket");
        child.ChildRecipes.Add(grandchild, 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        ComponentMap result = RecipeProcessor.Flatten(parent);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Rivet");
        result.ComponentList[0].Quantity.Should().Be(6); // 1 x3 Rivet Sets x2 Brackets
    }

    [Test]
    public void Flatten_Diamond_CombinesSharedComponentFromBothBranches()
    {
        Recipe left = NewRecipe("Left Arm");
        left.Components.Add(NewComponent("Bolt"), 1);

        Recipe right = NewRecipe("Right Arm");
        right.Components.Add(NewComponent("Bolt"), 1);

        Recipe parent = NewRecipe("Chassis");
        parent.ChildRecipes.Add(left, 1);
        parent.ChildRecipes.Add(right, 1);

        ComponentMap result = RecipeProcessor.Flatten(parent);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Bolt");
        result.ComponentList[0].Quantity.Should().Be(2);
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
        child.Components.Add(NewComponent("Screw"), 3);

        Recipe parent = NewRecipe("Frame");
        parent.ChildRecipes.Add(child, 2);

        RecipeNode tree = RecipeProcessor.BuildNode(parent, 1);

        tree.Name.Should().Be("Frame x1");
        RecipeNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Name.Should().Be("Bracket x2");
        childNode.IsComponent.Should().BeFalse();
        RecipeNode componentNode = childNode.Children.Should().ContainSingle().Subject;
        componentNode.Name.Should().Be("Screw x6");
        componentNode.IsComponent.Should().BeTrue();
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
