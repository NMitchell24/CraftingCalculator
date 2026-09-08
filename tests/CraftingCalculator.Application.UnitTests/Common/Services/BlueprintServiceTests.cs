using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class BlueprintServiceTests
{
    private Mock<IBlueprintDAO> _dao = null!;
    private BlueprintService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _dao = new Mock<IBlueprintDAO>();
        _service = new BlueprintService(_dao.Object);
    }

    [Test]
    public async Task GetBlueprintByIdAsync_DelegatesToDAO()
    {
        Blueprint blueprint = new Blueprint { Id = 5, Name = "Widget" };
        _dao.Setup(d => d.GetByIdAsync(5)).ReturnsAsync(blueprint);

        Blueprint? result = await _service.GetBlueprintByIdAsync(5);

        result.Should().BeSameAs(blueprint);
    }

    [Test]
    public async Task SaveBlueprintAsync_NullBlueprint_DoesNotCallDAO()
    {
        await _service.SaveBlueprintAsync(null);

        _dao.Verify(d => d.SaveAsync(It.IsAny<Blueprint>()), Times.Never);
    }

    [Test]
    public async Task SaveBlueprintAsync_SavesThroughDAO()
    {
        Blueprint blueprint = new Blueprint { Name = "Widget" };

        await _service.SaveBlueprintAsync(blueprint);

        _dao.Verify(d => d.SaveAsync(blueprint), Times.Once);
    }

    [Test]
    public async Task DeleteBlueprintAsync_DeletesById()
    {
        Blueprint blueprint = new Blueprint { Id = 7, Name = "Widget" };

        await _service.DeleteBlueprintAsync(blueprint);

        _dao.Verify(d => d.DeleteAsync(7), Times.Once);
    }

    [Test]
    public void GetFlattenedComponents_CombinesNestedChildComponents()
    {
        Blueprint child = new Blueprint { Name = "Bracket" };
        child.Components.Add(new Component { Name = "Screw" }, 3);

        Blueprint parent = new Blueprint { Name = "Frame" };
        parent.ChildBlueprints.Add(child, 2);

        (ComponentMap result, BlueprintMap surplus) = _service.GetFlattenedComponents(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6);
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void GetBlueprintNode_BuildsNodePerComponent()
    {
        Blueprint blueprint = new Blueprint { Name = "Widget" };
        blueprint.Components.Add(new Component { Name = "Screw" }, 1);

        BlueprintNode tree = _service.GetBlueprintNode(blueprint, 3);

        tree.Name.Should().Be("Widget x3");
        tree.Crafts.Should().Be(3);
        tree.Children.Should().ContainSingle();
    }
}
