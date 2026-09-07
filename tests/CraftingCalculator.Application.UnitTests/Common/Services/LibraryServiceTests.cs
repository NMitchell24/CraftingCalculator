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
public class LibraryServiceTests
{
    private Mock<IComponentService> _componentService = null!;
    private Mock<IRecipeFilterService> _recipeFilterService = null!;
    private Mock<IRecipeService> _recipeService = null!;
    private LibraryService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _componentService = new Mock<IComponentService>();
        _recipeFilterService = new Mock<IRecipeFilterService>();
        _recipeService = new Mock<IRecipeService>();
        _service = new LibraryService(_componentService.Object, _recipeFilterService.Object, _recipeService.Object);
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
    public async Task GetRecordsAsync_Recipe_ReturnsRecipes()
    {
        _recipeService.Setup(s => s.GetAllRecipesAsync())
            .ReturnsAsync([new Recipe { Id = 1, Name = "Widget" }]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.Recipe);

        records.Should().ContainSingle().Which.Name.Should().Be("Widget");
    }

    [Test]
    public async Task GetRecordsAsync_RecipeFilter_ExcludesTheSeededAllFilter()
    {
        _recipeFilterService.Setup(s => s.GetRecipeFiltersAsync()).ReturnsAsync(
        [
            new RecipeFilter { Id = DatabaseSeedConstants.AllFilterId, Name = RecipeFilter.ALL },
            new RecipeFilter { Id = 2, Name = "Tools" }
        ]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.RecipeFilter);

        records.Should().ContainSingle().Which.Name.Should().Be("Tools");
    }

    [Test]
    public async Task GetRecordsAsync_RecipeFilter_KeepsAUserCategoryNamedAll()
    {
        _recipeFilterService.Setup(s => s.GetRecipeFiltersAsync()).ReturnsAsync(
        [
            new RecipeFilter { Id = DatabaseSeedConstants.AllFilterId, Name = RecipeFilter.ALL },
            new RecipeFilter { Id = 7, Name = RecipeFilter.ALL }
        ]);

        List<IBaseDataRecord> records = await _service.GetRecordsAsync(DataType.RecipeFilter);

        records.Should().ContainSingle().Which.Id.Should().Be(7);
    }

    [Test]
    public async Task GetRecordAsync_RoutesToTheServiceMatchingTheType()
    {
        _recipeService.Setup(s => s.GetRecipeByIdAsync(5)).ReturnsAsync(new Recipe { Id = 5, Name = "Widget" });

        IBaseDataRecord? record = await _service.GetRecordAsync(DataType.Recipe, 5);

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
    public async Task SaveRecordAsync_RecipeFilter_SavesThroughTheRecipeFilterService()
    {
        RecipeFilter filter = new RecipeFilter { Id = 2, Name = "Tools" };

        await _service.SaveRecordAsync(filter);

        _recipeFilterService.Verify(s => s.SaveRecipeFilterAsync(filter), Times.Once);
    }

    [Test]
    public async Task SaveRecordAsync_Recipe_SavesThroughTheRecipeService()
    {
        Recipe recipe = new Recipe { Id = 4, Name = "Widget" };

        await _service.SaveRecordAsync(recipe);

        _recipeService.Verify(s => s.SaveRecipeAsync(recipe), Times.Once);
    }

    [Test]
    public async Task DeleteRecordAsync_Recipe_DeletesThroughTheRecipeService()
    {
        Recipe recipe = new Recipe { Id = 4, Name = "Widget" };

        await _service.DeleteRecordAsync(recipe);

        _recipeService.Verify(s => s.DeleteRecipeAsync(recipe), Times.Once);
        _componentService.Verify(s => s.DeleteComponentAsync(It.IsAny<Component>()), Times.Never);
    }

    [Test]
    public async Task SaveRecordAsync_Null_DoesNothing()
    {
        await _service.SaveRecordAsync(null);
        await _service.DeleteRecordAsync(null);

        _componentService.VerifyNoOtherCalls();
        _recipeFilterService.VerifyNoOtherCalls();
        _recipeService.VerifyNoOtherCalls();
    }
}
