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
        BlueprintModel blueprint = new BlueprintModel { Id = 5, Name = "Widget" };
        _dao.Setup(d => d.GetByIdAsync(5)).ReturnsAsync(blueprint);

        BlueprintModel? result = await _service.GetBlueprintByIdAsync(5);

        result.Should().BeSameAs(blueprint);
    }

    [Test]
    public async Task SaveBlueprintAsync_NullBlueprint_DoesNotCallDAO()
    {
        await _service.SaveBlueprintAsync(null);

        _dao.Verify(d => d.SaveAsync(It.IsAny<BlueprintModel>()), Times.Never);
    }

    [Test]
    public async Task SaveBlueprintAsync_SavesThroughDAO()
    {
        BlueprintModel blueprint = new BlueprintModel { Name = "Widget" };

        await _service.SaveBlueprintAsync(blueprint);

        _dao.Verify(d => d.SaveAsync(blueprint), Times.Once);
    }

    [Test]
    public async Task DeleteBlueprintAsync_DeletesById()
    {
        BlueprintModel blueprint = new BlueprintModel { Id = 7, Name = "Widget" };

        await _service.DeleteBlueprintAsync(blueprint);

        _dao.Verify(d => d.DeleteAsync(7), Times.Once);
    }

    [Test]
    public void GetBlueprintNode_BuildsNodePerComponent()
    {
        BlueprintModel blueprint = new BlueprintModel { Name = "Widget" };
        blueprint.Components.Add(new ComponentModel { Name = "Screw" }, 1);

        BlueprintNode tree = _service.GetBlueprintNode(blueprint, 3);

        tree.Name.Should().Be("Widget");
        tree.Quantity.Should().Be(3);
        tree.Crafts.Should().Be(3);
        tree.Children.Should().ContainSingle();
    }
}
