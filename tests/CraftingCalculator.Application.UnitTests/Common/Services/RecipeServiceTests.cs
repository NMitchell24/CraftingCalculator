using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class RecipeServiceTests
{
    private Mock<IRecipeDAO> _dao = null!;
    private RecipeService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _dao = new Mock<IRecipeDAO>();
        _service = new RecipeService(_dao.Object);
    }

    [Test]
    public async Task GetRecipeByIdAsync_DelegatesToDAO()
    {
        Recipe recipe = new Recipe { Id = 5, Name = "Widget" };
        _dao.Setup(d => d.GetByIdAsync(5)).ReturnsAsync(recipe);

        Recipe? result = await _service.GetRecipeByIdAsync(5);

        result.Should().BeSameAs(recipe);
    }

    [Test]
    public async Task SaveRecipeAsync_NullRecipe_DoesNotCallDAO()
    {
        await _service.SaveRecipeAsync(null);

        _dao.Verify(d => d.SaveAsync(It.IsAny<Recipe>()), Times.Never);
    }

    [Test]
    public async Task SaveRecipeAsync_SavesThroughDAO()
    {
        Recipe recipe = new Recipe { Name = "Widget" };

        await _service.SaveRecipeAsync(recipe);

        _dao.Verify(d => d.SaveAsync(recipe), Times.Once);
    }

    [Test]
    public async Task DeleteRecipeAsync_DeletesById()
    {
        Recipe recipe = new Recipe { Id = 7, Name = "Widget" };

        await _service.DeleteRecipeAsync(recipe);

        _dao.Verify(d => d.DeleteAsync(7), Times.Once);
    }

    [Test]
    public void GetFlattenedComponents_CombinesNestedChildComponents()
    {
        Recipe child = new Recipe { Name = "Bracket" };
        child.Components.Add(new Component { Name = "Screw" }, 3);

        Recipe parent = new Recipe { Name = "Frame" };
        parent.ChildRecipes.Add(child, 2);

        ComponentMap result = _service.GetFlattenedComponents(parent);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6);
    }

    [Test]
    public void GetRecipeNode_BuildsNodePerComponent()
    {
        Recipe recipe = new Recipe { Name = "Widget" };
        recipe.Components.Add(new Component { Name = "Screw" }, 1);

        RecipeNode tree = _service.GetRecipeNode(recipe, 3);

        tree.Name.Should().Be("Widget x3");
        tree.Children.Should().ContainSingle();
    }
}
