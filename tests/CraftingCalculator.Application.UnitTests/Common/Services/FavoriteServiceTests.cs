using AwesomeAssertions;
using CraftingCalculator.Application.Common.Interfaces.DAO;
using CraftingCalculator.Application.Common.Services.Impl;
using CraftingCalculator.Domain.Models;
using Moq;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Common.Services;

[TestFixture]
public class FavoriteServiceTests
{
    private Mock<IBlueprintFavoritesDAO> _dao = null!;
    private FavoriteService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _dao = new Mock<IBlueprintFavoritesDAO>();
        _service = new FavoriteService(_dao.Object);
    }

    [Test]
    public async Task SaveFavoriteAsync_SavesFavoriteAndItsQuantitiesThroughDAO()
    {
        BlueprintFavorite favorite = new BlueprintFavorite { Name = "My Batch" };
        List<BlueprintQuantity> quantities = [new BlueprintQuantity(new BlueprintModel { Name = "Widget" }, 2, 0)];

        await _service.SaveFavoriteAsync(favorite, quantities);

        _dao.Verify(d => d.SaveAsync(favorite, quantities), Times.Once);
    }

    [Test]
    public async Task RenameFavoriteAsync_RenamesByIdThroughDAO()
    {
        BlueprintFavorite favorite = new BlueprintFavorite { Id = 4, Name = "Old" };

        await _service.RenameFavoriteAsync(favorite, "New");

        _dao.Verify(d => d.RenameAsync(4, "New"), Times.Once);
    }

    [Test]
    public async Task DoesFavoriteExistAsync_KnownName_ReturnsTrue()
    {
        _dao.Setup(d => d.GetByNameAsync("My Batch")).ReturnsAsync(new BlueprintFavorite { Name = "My Batch" });

        bool exists = await _service.DoesFavoriteExistAsync("My Batch");

        exists.Should().BeTrue();
    }

    [Test]
    public async Task DoesFavoriteExistAsync_UnknownName_ReturnsFalse()
    {
        _dao.Setup(d => d.GetByNameAsync("Missing")).ReturnsAsync((BlueprintFavorite?)null);

        bool exists = await _service.DoesFavoriteExistAsync("Missing");

        exists.Should().BeFalse();
    }

    [Test]
    public async Task DoesFavoriteExistAsync_NullName_ReturnsTrue()
    {
        // Parity with the WPF-era bug: GetFavoriteByName(null) returns an empty favorite rather than
        // null, so a null name always reports as "exists". Reproduced deliberately - see the
        // "DoesFavoriteExist always reports true for a null name" migration issue.
        _dao.Setup(d => d.GetByNameAsync(null)).ReturnsAsync(new BlueprintFavorite());

        bool exists = await _service.DoesFavoriteExistAsync(null);

        exists.Should().BeTrue();
    }

    [Test]
    public async Task GetBlueprintQuantitiesForFavoriteAsync_NullFavorite_ReturnsEmptyListWithoutCallingDAO()
    {
        List<BlueprintQuantity> result = await _service.GetBlueprintQuantitiesForFavoriteAsync(null);

        result.Should().BeEmpty();
        _dao.Verify(d => d.GetBlueprintQuantitiesAsync(It.IsAny<int>()), Times.Never);
    }
}
