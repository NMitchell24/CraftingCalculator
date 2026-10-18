using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class BlueprintFavoritesDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private BlueprintFavoritesDAO _favoritesDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _blueprintDAO = new BlueprintDAO(_fixture.DatasetFactory);
        _favoritesDAO = new BlueprintFavoritesDAO(_fixture.DatasetFactory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task GetAllAsync_ReturnsEachFavoritesSavedBlueprintCount()
    {
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });
        BlueprintModel table = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Table" });

        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Two" },
            new Batch([new BlueprintQuantity(plank, 3), new BlueprintQuantity(table, 1)], new HashSet<int>()));
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "One" }, new Batch([new BlueprintQuantity(plank, 5)], new HashSet<int>()));
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Empty" }, Batch.Empty);

        List<BlueprintFavorite> favorites = await _favoritesDAO.GetAllAsync();

        favorites.Select(f => f.Name).Should().Equal("Empty", "One", "Two");
        favorites.Select(f => f.BlueprintCount).Should().Equal(0, 1, 2);
    }

    [Test]
    public async Task GetBatchAsync_BuildsEachBlueprintOut_InTheOrderTheyWereSaved()
    {
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });

        BlueprintModel nails = new() { Name = "Bronze Nails" };
        nails.ChildBlueprints.Add(bronze, 1);
        nails = await _blueprintDAO.SaveAsync(nails);

        BlueprintModel axe = new() { Name = "Bronze Axe" };
        axe.ChildBlueprints.Add(bronze, 8);
        axe = await _blueprintDAO.SaveAsync(axe);

        BlueprintFavorite prep = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Karve prep" },
            new Batch([new BlueprintQuantity(nails, 10), new BlueprintQuantity(axe, 1)], new HashSet<int>()));

        IReadOnlyList<BlueprintQuantity> quantities = (await _favoritesDAO.GetBatchAsync(prep.Id)).Blueprints;

        quantities.Select(q => (q.Blueprint.Name, q.Quantity)).Should().Equal(("Bronze Nails", 10L), ("Bronze Axe", 1L));
        quantities.Should().AllSatisfy(q => q.Blueprint.ChildBlueprints.BlueprintList.Single().Name.Should().Be("Bronze"));
    }

    [Test]
    public async Task GetBatchAsync_ReadsBackTheBlueprintsSavedAsAlreadyCrafted()
    {
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });

        BlueprintModel axe = new() { Name = "Bronze Axe" };
        axe.ChildBlueprints.Add(bronze, 8);
        axe = await _blueprintDAO.SaveAsync(axe);

        BlueprintFavorite run = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Bronze Axe run" },
            new Batch([new BlueprintQuantity(axe, 5)], new HashSet<int> { bronze.Id }));

        Batch batch = await _favoritesDAO.GetBatchAsync(run.Id);

        batch.Blueprints.Select(q => (q.Blueprint.Name, q.Quantity)).Should().Equal(("Bronze Axe", 5L));
        batch.AlreadyCraftedIds.Should().BeEquivalentTo([bronze.Id]);
    }

    [Test]
    public async Task SaveAsync_AgainReplacesTheBlueprintsSavedAsAlreadyCrafted()
    {
        // Valheim: a Karve takes 80 Bronze Nails, and one Bronze makes 20 of them.
        BlueprintModel bronze = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bronze" });

        BlueprintModel nails = new() { Name = "Bronze Nails", Yield = 20 };
        nails.ChildBlueprints.Add(bronze, 1);
        nails = await _blueprintDAO.SaveAsync(nails);

        BlueprintModel karve = new() { Name = "Karve" };
        karve.ChildBlueprints.Add(nails, 80);
        karve = await _blueprintDAO.SaveAsync(karve);

        List<BlueprintQuantity> entries = [new(karve, 1)];
        BlueprintFavorite saved = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Karve" },
            new Batch(entries, new HashSet<int> { bronze.Id }));

        await _favoritesDAO.SaveAsync(saved, new Batch(entries, new HashSet<int> { nails.Id }));
        (await _favoritesDAO.GetBatchAsync(saved.Id)).AlreadyCraftedIds.Should().BeEquivalentTo([nails.Id]);

        await _favoritesDAO.SaveAsync(saved, new Batch(entries, new HashSet<int>()));
        (await _favoritesDAO.GetBatchAsync(saved.Id)).AlreadyCraftedIds.Should().BeEmpty();
    }

    [Test]
    public async Task RenameAsync_ChangesTheNameAndLeavesTheSavedQuantitiesIntact()
    {
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });
        BlueprintFavorite saved = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Old" },
            new Batch([new BlueprintQuantity(plank, 7)], new HashSet<int>()));

        await _favoritesDAO.RenameAsync(saved.Id, "New");

        (await _favoritesDAO.GetByNameAsync("Old")).Should().BeNull();
        (await _favoritesDAO.GetByNameAsync("New")).Should().NotBeNull();

        IReadOnlyList<BlueprintQuantity> quantities = (await _favoritesDAO.GetBatchAsync(saved.Id)).Blueprints;
        quantities.Should().ContainSingle(q => q.Blueprint.Name == "Plank" && q.Quantity == 7);
    }

    [Test]
    public async Task DeleteAsync_DeletesEveryNamedFavoriteAndCascadesToItsSavedQuantities()
    {
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        BlueprintFavorite one = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "One" },
            new Batch([new BlueprintQuantity(plank, 3)], new HashSet<int>()));
        BlueprintFavorite two = await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Two" },
            new Batch([new BlueprintQuantity(plank, 5)], new HashSet<int>()));
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Kept" }, new Batch([new BlueprintQuantity(plank, 7)], new HashSet<int>()));

        await _favoritesDAO.DeleteAsync([one.Id, two.Id]);

        (await _favoritesDAO.GetAllAsync()).Select(f => f.Name).Should().Equal("Kept");
        (await _favoritesDAO.GetBatchAsync(one.Id)).Blueprints.Should().BeEmpty();
        (await _favoritesDAO.GetBatchAsync(two.Id)).Blueprints.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_EmptyIdList_DeletesNothing()
    {
        await _favoritesDAO.SaveAsync(new BlueprintFavorite { Name = "Kept" }, Batch.Empty);

        await _favoritesDAO.DeleteAsync([]);

        (await _favoritesDAO.GetAllAsync()).Should().ContainSingle();
    }
}
