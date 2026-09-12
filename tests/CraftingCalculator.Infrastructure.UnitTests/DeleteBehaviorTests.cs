using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
using CraftingCalculator.Infrastructure.DAO.Impl;
using Microsoft.EntityFrameworkCore;

namespace CraftingCalculator.Infrastructure.UnitTests;

/// <summary>
/// One test per row of the plan's delete-behavior table: each cascade/SetNull configuration replaces
/// a hand-written cleanup loop that used to walk LiteDB collections, so these are the direct
/// regression net for that removed code.
/// </summary>
[TestFixture]
public class DeleteBehaviorTests
{
    private SqliteTestFixture _fixture = null!;

    [SetUp]
    public void SetUp() => _fixture = new SqliteTestFixture();

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task DeletingBlueprint_CascadesItsOwnComponents()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Blueprint blueprint = new() { Name = "Plank" };
        blueprint.Components.Add(new BlueprintComponent { Component = new Component { Name = "Wood" }, Quantity = 2 });
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Blueprints.Where(bp => bp.Id == blueprint.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintComponents.CountAsync(blueprintComponent => blueprintComponent.BlueprintId == blueprint.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingComponent_CascadesBlueprintComponentsReferencingIt()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Component component = new() { Name = "Wood" };
        Blueprint blueprint = new() { Name = "Plank" };
        blueprint.Components.Add(new BlueprintComponent { Component = component, Quantity = 2 });
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Components.Where(candidate => candidate.Id == component.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintComponents.CountAsync(blueprintComponent => blueprintComponent.ComponentId == component.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingParentBlueprint_CascadesItsChildLinks()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Blueprint child = new() { Name = "Plank" };
        Blueprint parent = new() { Name = "Table" };
        parent.Children.Add(new BlueprintChild { Child = child, Quantity = 4 });
        seed.Blueprints.AddRange(parent, child);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Blueprints.Where(blueprint => blueprint.Id == parent.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintChildren.CountAsync(blueprintChild => blueprintChild.ParentBlueprintId == parent.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingChildBlueprint_CascadesLinksFromBlueprintsThatUseItAsAComponent()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Blueprint child = new() { Name = "Plank" };
        Blueprint parent = new() { Name = "Table" };
        parent.Children.Add(new BlueprintChild { Child = child, Quantity = 4 });
        seed.Blueprints.AddRange(parent, child);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Blueprints.Where(blueprint => blueprint.Id == child.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintChildren.CountAsync(blueprintChild => blueprintChild.ChildBlueprintId == child.Id)).Should().Be(0);
        // The parent blueprint itself must survive - only the link to its now-deleted component goes.
        (await verify.Blueprints.AnyAsync(blueprint => blueprint.Id == parent.Id)).Should().BeTrue();
    }

    [Test]
    public async Task DeletingCategory_SetsBlueprintsCategoryIdToNullRatherThanDeletingThem()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Category category = new() { Name = "Building" };
        Blueprint blueprint = new() { Name = "Table", Category = category };
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Categories.Where(candidate => candidate.Id == category.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        Blueprint? reloaded = await verify.Blueprints.FirstOrDefaultAsync(candidate => candidate.Id == blueprint.Id);
        reloaded.Should().NotBeNull();
        reloaded!.CategoryId.Should().BeNull();
    }

    [Test]
    public async Task DeletingCategory_SetsComponentsCategoryIdToNullRatherThanDeletingThem()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Category category = new() { Name = "Ores" };
        Component component = new() { Name = "Iron", Category = category };
        seed.Components.Add(component);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Categories.Where(candidate => candidate.Id == category.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        Component? reloaded = await verify.Components.FirstOrDefaultAsync(candidate => candidate.Id == component.Id);
        reloaded.Should().NotBeNull();
        reloaded!.CategoryId.Should().BeNull();
    }

    [Test]
    public async Task DeletingFavorite_CascadesItsSavedBlueprintQuantities()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Blueprint blueprint = new() { Name = "Table" };
        Favorite favorite = new() { Name = "Weekly Batch" };
        favorite.FavoriteBlueprints.Add(new FavoriteBlueprint { Blueprint = blueprint, Quantity = 3 });
        seed.Favorites.Add(favorite);
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Favorites.Where(f => f.Id == favorite.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.FavoriteBlueprints.CountAsync(fr => fr.FavoriteId == favorite.Id)).Should().Be(0);
    }

    [Test]
    public async Task DeletingBlueprint_CascadesFavoriteEntriesReferencingIt()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        Blueprint blueprint = new() { Name = "Table" };
        Favorite favorite = new() { Name = "Weekly Batch" };
        favorite.FavoriteBlueprints.Add(new FavoriteBlueprint { Blueprint = blueprint, Quantity = 3 });
        seed.Favorites.Add(favorite);
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.Blueprints.Where(bp => bp.Id == blueprint.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.FavoriteBlueprints.CountAsync(fr => fr.BlueprintId == blueprint.Id)).Should().Be(0);
        // The favorite itself survives - only its entry for the deleted blueprint goes.
        (await verify.Favorites.AnyAsync(f => f.Id == favorite.Id)).Should().BeTrue();
    }

    [Test]
    public async Task DeletingDataset_CascadesEveryRecordFiledUnderIt()
    {
        DatasetDAO datasetDAO = new(_fixture.RawFactory);
        int rustId = (await datasetDAO.AddAsync("Rust")).Id;

        _fixture.SelectDataset(rustId);

        await using (CraftingDataContext seed = _fixture.Factory.CreateDbContext())
        {
            Category tools = new() { Name = "Tools" };
            Component wood = new() { Name = "Wood" };
            Blueprint arrow = new() { Name = "Arrow", Category = tools };
            arrow.Components.Add(new BlueprintComponent { Component = wood, Quantity = 25 });
            Favorite raidKit = new() { Name = "Raid kit" };
            raidKit.FavoriteBlueprints.Add(new FavoriteBlueprint { Blueprint = arrow, Quantity = 2 });

            seed.Favorites.Add(raidKit);
            seed.Blueprints.Add(arrow);
            await seed.SaveChangesAsync();
        }

        await datasetDAO.DeleteAsync(rustId);

        // Read unfiltered: a filtered read would come back empty whether the rows were deleted or
        // merely hidden, which is the one thing this test must be able to tell apart.
        await using CraftingDataContext verify = _fixture.RawFactory.CreateDbContext();
        (await verify.Categories.IgnoreQueryFilters().CountAsync(c => c.DatasetId == rustId)).Should().Be(0);
        (await verify.Components.IgnoreQueryFilters().CountAsync(c => c.DatasetId == rustId)).Should().Be(0);
        (await verify.Blueprints.IgnoreQueryFilters().CountAsync(b => b.DatasetId == rustId)).Should().Be(0);
        (await verify.Favorites.IgnoreQueryFilters().CountAsync(f => f.DatasetId == rustId)).Should().Be(0);

        // And the link rows their parents owned went with them.
        (await verify.BlueprintComponents.IgnoreQueryFilters().CountAsync()).Should().Be(0);
        (await verify.FavoriteBlueprints.IgnoreQueryFilters().CountAsync()).Should().Be(0);
    }
}
