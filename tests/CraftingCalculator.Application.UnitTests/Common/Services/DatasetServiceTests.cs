using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Constants;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class DatasetServiceTests
{
    private Mock<IComponentService> _componentService = null!;
    private Mock<ICategoryService> _categoryService = null!;
    private Mock<IBlueprintService> _blueprintService = null!;
    private DatasetService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _componentService = new Mock<IComponentService>();
        _categoryService = new Mock<ICategoryService>();
        _blueprintService = new Mock<IBlueprintService>();
        _service = new DatasetService(_componentService.Object, _categoryService.Object, _blueprintService.Object);
    }

    [Test]
    public async Task GetRecordsAsync_Component_ReturnsComponents()
    {
        _componentService.Setup(s => s.GetAllComponentsAsync())
            .ReturnsAsync([new Component { Id = 1, Name = "Screw" }]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Component);

        records.Should().ContainSingle().Which.Name.Should().Be("Screw");
    }

    [Test]
    public async Task GetRecordsAsync_Blueprint_ReturnsBlueprints()
    {
        _blueprintService.Setup(s => s.GetAllBlueprintsAsync())
            .ReturnsAsync([new Blueprint { Id = 1, Name = "Widget" }]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Blueprint);

        records.Should().ContainSingle().Which.Name.Should().Be("Widget");
    }

    [Test]
    public async Task GetRecordsAsync_Category_ExcludesTheSeededAllCategory()
    {
        _categoryService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(
        [
            new Category { Id = DatabaseSeedConstants.AllCategoryId, Name = Category.ALL },
            new Category { Id = 2, Name = "Tools" }
        ]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Category);

        records.Should().ContainSingle().Which.Name.Should().Be("Tools");
    }

    [Test]
    public async Task GetRecordsAsync_Category_KeepsAUserCategoryNamedAll()
    {
        _categoryService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(
        [
            new Category { Id = DatabaseSeedConstants.AllCategoryId, Name = Category.ALL },
            new Category { Id = 7, Name = Category.ALL }
        ]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Category);

        records.Should().ContainSingle().Which.Id.Should().Be(7);
    }

    [Test]
    public async Task GetRecordAsync_RoutesToTheServiceMatchingTheType()
    {
        _blueprintService.Setup(s => s.GetBlueprintByIdAsync(5)).ReturnsAsync(new Blueprint { Id = 5, Name = "Widget" });

        IBaseDataRecord? record = await _service.GetRecordAsync(DataType.Blueprint, 5);

        record!.Name.Should().Be("Widget");
        _componentService.Verify(s => s.GetComponentByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task SaveRecordAsync_Component_SavesThroughTheComponentService()
    {
        Component component = new Component { Id = 3, Name = "Screw" };

        await _service.SaveRecordAsync(component);

        _componentService.Verify(s => s.SaveComponentAsync(component), Times.Once);
    }

    [Test]
    public async Task SaveRecordAsync_Category_SavesThroughTheCategoryService()
    {
        Category category = new Category { Id = 2, Name = "Tools" };

        await _service.SaveRecordAsync(category);

        _categoryService.Verify(s => s.SaveCategoryAsync(category), Times.Once);
    }

    [Test]
    public async Task SaveRecordAsync_Blueprint_SavesThroughTheBlueprintService()
    {
        Blueprint blueprint = new Blueprint { Id = 4, Name = "Widget" };

        await _service.SaveRecordAsync(blueprint);

        _blueprintService.Verify(s => s.SaveBlueprintAsync(blueprint), Times.Once);
    }

    [Test]
    public async Task DeleteRecordAsync_Blueprint_DeletesThroughTheBlueprintService()
    {
        Blueprint blueprint = new Blueprint { Id = 4, Name = "Widget" };

        await _service.DeleteRecordAsync(blueprint);

        _blueprintService.Verify(s => s.DeleteBlueprintAsync(blueprint), Times.Once);
        _componentService.Verify(s => s.DeleteComponentAsync(It.IsAny<Component>()), Times.Never);
    }

    [Test]
    public async Task DeleteRecordsAsync_DeletesEachRecordThroughItsOwnService()
    {
        Component component = new Component { Id = 1, Name = "Screw" };
        Category category = new Category { Id = 2, Name = "Tools" };
        Blueprint blueprint = new Blueprint { Id = 3, Name = "Widget" };

        await _service.DeleteRecordsAsync([component, category, blueprint]);

        _componentService.Verify(s => s.DeleteComponentAsync(component), Times.Once);
        _categoryService.Verify(s => s.DeleteCategoryAsync(category), Times.Once);
        _blueprintService.Verify(s => s.DeleteBlueprintAsync(blueprint), Times.Once);
    }

    [Test]
    public async Task DeleteRecordsAsync_Empty_DoesNothing()
    {
        await _service.DeleteRecordsAsync([]);

        _componentService.VerifyNoOtherCalls();
        _categoryService.VerifyNoOtherCalls();
        _blueprintService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteAllOfTypeAsync_Component_DeletesEveryComponent()
    {
        Component screw = new Component { Id = 1, Name = "Screw" };
        Component bolt = new Component { Id = 2, Name = "Bolt" };
        _componentService.Setup(s => s.GetAllComponentsAsync()).ReturnsAsync([screw, bolt]);

        await _service.DeleteAllOfTypeAsync(DataType.Component);

        _componentService.Verify(s => s.DeleteComponentAsync(screw), Times.Once);
        _componentService.Verify(s => s.DeleteComponentAsync(bolt), Times.Once);
    }

    [Test]
    public async Task DeleteAllOfTypeAsync_Category_KeepsTheSeededAllCategory()
    {
        Category all = new Category { Id = DatabaseSeedConstants.AllCategoryId, Name = Category.ALL };
        Category tools = new Category { Id = 2, Name = "Tools" };
        _categoryService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync([all, tools]);

        await _service.DeleteAllOfTypeAsync(DataType.Category);

        _categoryService.Verify(s => s.DeleteCategoryAsync(tools), Times.Once);
        _categoryService.Verify(s => s.DeleteCategoryAsync(all), Times.Never);
    }

    [Test]
    public async Task SaveRecordAsync_Null_DoesNothing()
    {
        await _service.SaveRecordAsync(null);
        await _service.DeleteRecordAsync(null);

        _componentService.VerifyNoOtherCalls();
        _categoryService.VerifyNoOtherCalls();
        _blueprintService.VerifyNoOtherCalls();
    }
}
