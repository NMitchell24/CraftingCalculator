using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class RecordServiceTests
{
    private Mock<IComponentService> _componentService = null!;
    private Mock<ICategoryService> _categoryService = null!;
    private Mock<IBlueprintService> _blueprintService = null!;
    private RecordService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _componentService = new Mock<IComponentService>();
        _categoryService = new Mock<ICategoryService>();
        _blueprintService = new Mock<IBlueprintService>();
        _service = new RecordService(_componentService.Object, _categoryService.Object, _blueprintService.Object);
    }

    [Test]
    public async Task GetRecordsAsync_Component_ReturnsComponents()
    {
        _componentService.Setup(s => s.GetAllComponentsAsync())
            .ReturnsAsync([new ComponentModel { Id = 1, Name = "Screw" }]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Component);

        records.Should().ContainSingle().Which.Name.Should().Be("Screw");
    }

    [Test]
    public async Task GetRecordsAsync_Blueprint_ReturnsSummaries()
    {
        _blueprintService.Setup(s => s.GetBlueprintSummariesAsync())
            .ReturnsAsync([new BlueprintSummary { Id = 1, Name = "Widget" }]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Blueprint);

        records.Should().ContainSingle().Which.Name.Should().Be("Widget");
    }

    [Test]
    public async Task CountRecordsAsync_RoutesToTheServiceMatchingTheType()
    {
        _componentService.Setup(s => s.CountComponentsAsync()).ReturnsAsync(12);
        _categoryService.Setup(s => s.CountCategoriesAsync()).ReturnsAsync(3);
        _blueprintService.Setup(s => s.CountBlueprintsAsync()).ReturnsAsync(7);

        (await _service.CountRecordsAsync(DataType.Component)).Should().Be(12);
        (await _service.CountRecordsAsync(DataType.Category)).Should().Be(3);
        (await _service.CountRecordsAsync(DataType.Blueprint)).Should().Be(7);
    }

    [Test]
    public async Task GetRecordsAsync_Category_ReturnsCategories()
    {
        _categoryService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(
        [
            new CategoryModel { Id = 2, Name = "Tools" },
            new CategoryModel { Id = 7, Name = "All" }
        ]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Category);

        records.Select(record => record.Name).Should().Equal("Tools", "All");
    }

    [Test]
    public async Task GetRecordAsync_RoutesToTheServiceMatchingTheType()
    {
        _blueprintService.Setup(s => s.GetBlueprintByIdAsync(5)).ReturnsAsync(new BlueprintModel { Id = 5, Name = "Widget" });

        IBaseDataRecord? record = await _service.GetRecordAsync(DataType.Blueprint, 5);

        record!.Name.Should().Be("Widget");
        _componentService.Verify(s => s.GetComponentByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task SaveRecordAsync_Component_SavesThroughTheComponentService()
    {
        ComponentModel component = new() { Id = 3, Name = "Screw" };

        await _service.SaveRecordAsync(component);

        _componentService.Verify(s => s.SaveComponentAsync(component), Times.Once);
    }

    [Test]
    public async Task SaveRecordAsync_Category_SavesThroughTheCategoryService()
    {
        CategoryModel category = new() { Id = 2, Name = "Tools" };

        await _service.SaveRecordAsync(category);

        _categoryService.Verify(s => s.SaveCategoryAsync(category), Times.Once);
    }

    [Test]
    public async Task SaveRecordAsync_Blueprint_SavesThroughTheBlueprintService()
    {
        BlueprintModel blueprint = new() { Id = 4, Name = "Widget" };

        await _service.SaveRecordAsync(blueprint);

        _blueprintService.Verify(s => s.SaveBlueprintAsync(blueprint), Times.Once);
    }

    [Test]
    public async Task DeleteRecordAsync_Blueprint_DeletesThroughTheBlueprintService()
    {
        BlueprintModel blueprint = new() { Id = 4, Name = "Widget" };

        await _service.DeleteRecordAsync(blueprint);

        _blueprintService.Verify(s => s.DeleteBlueprintsAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 4 }))), Times.Once);
        _componentService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DeleteRecordAsync_Component_DeletesThroughTheComponentService()
    {
        await _service.DeleteRecordAsync(new ComponentModel { Id = 1, Name = "Screw" });

        _componentService.Verify(s => s.DeleteComponentsAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 1 }))), Times.Once);
    }

    [Test]
    public async Task DeleteRecordAsync_BlueprintSummary_DeletesThroughTheBlueprintService()
    {
        await _service.DeleteRecordAsync(new BlueprintSummary { Id = 4, Name = "Widget" });

        _blueprintService.Verify(s => s.DeleteBlueprintsAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 4 }))), Times.Once);
    }

    [TestCase(DataType.Component)]
    [TestCase(DataType.Category)]
    [TestCase(DataType.Blueprint)]
    public async Task DeleteRecordsAsync_HandsTheIdsToThatTypesServiceInOneCall(DataType type)
    {
        int[] ids = [2, 3];

        await _service.DeleteRecordsAsync(type, ids);

        switch (type)
        {
            case DataType.Component:
                _componentService.Verify(s => s.DeleteComponentsAsync(ids), Times.Once);
                break;
            case DataType.Category:
                _categoryService.Verify(s => s.DeleteCategoriesAsync(ids), Times.Once);
                break;
            case DataType.Blueprint:
                _blueprintService.Verify(s => s.DeleteBlueprintsAsync(ids), Times.Once);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        _componentService.VerifyNoOtherCalls();
        _categoryService.VerifyNoOtherCalls();
        _blueprintService.VerifyNoOtherCalls();
    }

    [TestCase(DataType.Component)]
    [TestCase(DataType.Category)]
    [TestCase(DataType.Blueprint)]
    public async Task DeleteAllOfTypeAsync_DeletesThroughThatTypesServiceWithoutReadingTheRecords(DataType type)
    {
        await _service.DeleteAllOfTypeAsync(type);

        switch (type)
        {
            case DataType.Component:
                _componentService.Verify(s => s.DeleteAllComponentsAsync(), Times.Once);
                break;
            case DataType.Category:
                _categoryService.Verify(s => s.DeleteAllCategoriesAsync(), Times.Once);
                break;
            case DataType.Blueprint:
                _blueprintService.Verify(s => s.DeleteAllBlueprintsAsync(), Times.Once);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        _componentService.VerifyNoOtherCalls();
        _categoryService.VerifyNoOtherCalls();
        _blueprintService.VerifyNoOtherCalls();
    }

    [Test]
    public async Task GetCopyAsync_ReturnsTheRecordUnsaved_WithItsPartsAndTheCopySuffix()
    {
        ComponentModel copper = new() { Id = 9, Name = "Copper" };
        BlueprintModel bronze = new() { Id = 5, Name = "Bronze", Yield = 3 };
        bronze.Components.Add(copper, 2);
        _blueprintService.Setup(s => s.GetBlueprintByIdAsync(5)).ReturnsAsync(bronze);

        BlueprintModel copy = (BlueprintModel)(await _service.GetCopyAsync(DataType.Blueprint, 5))!;

        copy.Id.Should().Be(0);
        copy.Name.Should().Be("Bronze - Copy");
        copy.Yield.Should().Be(3);
        copy.Components.ComponentList.Should().ContainSingle(part => part.Component.Id == 9 && part.Quantity == 2);
    }

    [Test]
    public async Task GetCopyAsync_NoRecordWithThatId_ReturnsNull()
    {
        (await _service.GetCopyAsync(DataType.Component, 404)).Should().BeNull();
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
