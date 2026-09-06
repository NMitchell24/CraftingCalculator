using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class IngredientProcessorTests
{
    [Test]
    public void CombineIngredients_AddsSourceQuantitiesOntoDestination()
    {
        Ingredient ingredient = new Ingredient { Id = 1, Name = "Test", Description = "Test" };

        IngredientMap source = new IngredientMap();
        source.Add(ingredient, 5);

        IngredientMap dest = new IngredientMap();
        dest.Add(ingredient, 10);

        IngredientMap result = IngredientProcessor.CombineIngredients(source, dest, 1);

        result.IngredientList.Should().ContainSingle();
        result.IngredientList[0].Ingredient.Name.Should().Be(ingredient.Name);
        result.IngredientList[0].Quantity.Should().Be(15);
    }

    [Test]
    public void CombineIngredients_MultipliesSourceQuantityBeforeAdding()
    {
        Ingredient ingredient = new Ingredient { Id = 1, Name = "Test" };

        IngredientMap source = new IngredientMap();
        source.Add(ingredient, 5);

        IngredientMap dest = new IngredientMap();

        IngredientMap result = IngredientProcessor.CombineIngredients(source, dest, 3);

        result.IngredientList.Should().ContainSingle();
        result.IngredientList[0].Quantity.Should().Be(15);
    }
}
