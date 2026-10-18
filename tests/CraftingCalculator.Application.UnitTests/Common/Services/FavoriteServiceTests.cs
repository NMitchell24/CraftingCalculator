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
    public async Task SaveFavoriteAsync_SavesFavoriteAndItsBatchThroughDAO()
    {
        BlueprintFavorite favorite = new() { Name = "My Batch" };
        Batch batch = new([new BlueprintQuantity(new BlueprintModel { Name = "Widget" }, 2)], new HashSet<int> { 7 });

        await _service.SaveFavoriteAsync(favorite, batch);

        _dao.Verify(d => d.SaveAsync(favorite, batch), Times.Once);
    }

    [Test]
    public async Task SaveFavoriteAsync_ReturnsTheFavoriteTheDAOSaved()
    {
        BlueprintFavorite favorite = new() { Name = "My Batch" };
        BlueprintFavorite saved = new() { Id = 9, Name = "My Batch" };
        _dao.Setup(d => d.SaveAsync(favorite, It.IsAny<Batch>())).ReturnsAsync(saved);

        BlueprintFavorite result = await _service.SaveFavoriteAsync(favorite, Batch.Empty);

        result.Should().BeSameAs(saved);
    }

    [Test]
    public async Task RenameFavoriteAsync_RenamesByIdThroughDAO()
    {
        BlueprintFavorite favorite = new() { Id = 4, Name = "Old" };

        await _service.RenameFavoriteAsync(favorite, "New");

        _dao.Verify(d => d.RenameAsync(4, "New"), Times.Once);
    }

    [Test]
    public async Task DeleteFavoriteAsync_DeletesThatOneIdThroughDAO()
    {
        await _service.DeleteFavoriteAsync(new BlueprintFavorite { Id = 4, Name = "Raid Kit" });

        _dao.Verify(d => d.DeleteAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 4 }))), Times.Once);
    }

    [Test]
    public async Task DeleteFavoritesAsync_DeletesEveryIdThroughOneDAOCall()
    {
        List<BlueprintFavorite> favorites =
        [
            new() { Id = 2, Name = "Raid Kit" },
            new() { Id = 7, Name = "Base Kit" }
        ];

        await _service.DeleteFavoritesAsync(favorites);

        _dao.Verify(d => d.DeleteAsync(It.Is<IEnumerable<int>>(ids => ids.SequenceEqual(new[] { 2, 7 }))), Times.Once);
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
    public async Task GetBatchForFavoriteAsync_NullFavorite_ReturnsTheEmptyBatchWithoutCallingDAO()
    {
        Batch result = await _service.GetBatchForFavoriteAsync(null);

        result.Should().BeSameAs(Batch.Empty);
        _dao.Verify(d => d.GetBatchAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public async Task GetBatchForFavoriteAsync_ReadsThatFavoritesBatchThroughDAO()
    {
        Batch saved = new([], new HashSet<int> { 3 });
        _dao.Setup(d => d.GetBatchAsync(4)).ReturnsAsync(saved);

        Batch result = await _service.GetBatchForFavoriteAsync(new BlueprintFavorite { Id = 4, Name = "Raid Kit" });

        result.Should().BeSameAs(saved);
    }
}
