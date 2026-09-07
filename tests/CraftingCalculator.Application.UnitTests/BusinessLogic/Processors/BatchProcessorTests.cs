using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BatchProcessorTests
{
    private static Component NewComponent(string name, double cost) => new Component { Id = 1, Name = name, Cost = cost };

    private static Recipe NewRecipe(string name, double value) => new Recipe { Id = 1, Name = name, Value = value };

    [Test]
    public void CalculateTotals_SingleRecipe_ReturnsCostAndValue()
    {
        Recipe recipe = NewRecipe("Widget", 10);
        recipe.Components.Add(NewComponent("Screw", 0.5), 2);

        RecipeQuantity batchEntry = new(recipe, 3, id: 0);

        (double totalCost, double totalValue, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(3.0); // 0.5 x2 Screws x3 Widgets
        totalValue.Should().Be(30); // 10 x3 Widgets
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
    }

    [Test]
    public void CalculateTotals_NestedRecipe_ScalesChildComponentsByBatchAndParentQuantity()
    {
        Recipe child = NewRecipe("Bracket", 0);
        child.Components.Add(NewComponent("Screw", 1), 3);

        Recipe parent = NewRecipe("Frame", 0);
        parent.ChildRecipes.Add(child, 2);

        RecipeQuantity batchEntry = new(parent, 2, id: 0);

        (double totalCost, _, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(12); // 1 x3 Screws x2 Brackets x2 Frames
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(12);
    }

    [Test]
    public void CalculateTotals_TwoRecipesSharingComponent_MergesMaterialsAndSumsValue()
    {
        Recipe left = NewRecipe("Left Arm", 5);
        left.Components.Add(NewComponent("Bolt", 1), 1);

        Recipe right = NewRecipe("Right Arm", 7);
        right.Components.Add(NewComponent("Bolt", 1), 1);

        RecipeQuantity leftEntry = new(left, 1, id: 0);
        RecipeQuantity rightEntry = new(right, 1, id: 0);

        (double totalCost, double totalValue, ComponentMap materials) =
            BatchProcessor.CalculateTotals([leftEntry, rightEntry]);

        totalCost.Should().Be(2);
        totalValue.Should().Be(12);
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_ZeroCostComponent_ContributesNothingToCost()
    {
        Recipe recipe = NewRecipe("Widget", 0);
        recipe.Components.Add(NewComponent("Free Sample", 0), 5);

        RecipeQuantity batchEntry = new(recipe, 1, id: 0);

        (double totalCost, _, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(0);
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(5);
    }
}
