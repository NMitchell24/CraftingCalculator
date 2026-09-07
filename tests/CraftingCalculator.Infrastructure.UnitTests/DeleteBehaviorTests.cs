using AwesomeAssertions;
using CraftingCalculator.Domain.Entities;
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
        await act.Blueprints.Where(r => r.Id == blueprint.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintComponents.CountAsync(ri => ri.BlueprintId == blueprint.Id)).Should().Be(0);
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
        await act.Components.Where(i => i.Id == component.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintComponents.CountAsync(ri => ri.ComponentId == component.Id)).Should().Be(0);
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
        await act.Blueprints.Where(r => r.Id == parent.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintChildren.CountAsync(rc => rc.ParentBlueprintId == parent.Id)).Should().Be(0);
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
        await act.Blueprints.Where(r => r.Id == child.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.BlueprintChildren.CountAsync(rc => rc.ChildBlueprintId == child.Id)).Should().Be(0);
        // The parent blueprint itself must survive - only the link to its now-deleted component goes.
        (await verify.Blueprints.AnyAsync(r => r.Id == parent.Id)).Should().BeTrue();
    }

    [Test]
    public async Task DeletingFilter_SetsBlueprintsFilterIdToNullRatherThanDeletingThem()
    {
        await using CraftingDataContext seed = _fixture.Factory.CreateDbContext();
        BlueprintFilter filter = new() { Name = "Building" };
        Blueprint blueprint = new() { Name = "Table", Filter = filter };
        seed.Blueprints.Add(blueprint);
        await seed.SaveChangesAsync();

        await using CraftingDataContext act = _fixture.Factory.CreateDbContext();
        await act.BlueprintFilters.Where(f => f.Id == filter.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        Blueprint? reloaded = await verify.Blueprints.FirstOrDefaultAsync(r => r.Id == blueprint.Id);
        reloaded.Should().NotBeNull();
        reloaded!.FilterId.Should().BeNull();
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
        await act.Blueprints.Where(r => r.Id == blueprint.Id).ExecuteDeleteAsync();

        await using CraftingDataContext verify = _fixture.Factory.CreateDbContext();
        (await verify.FavoriteBlueprints.CountAsync(fr => fr.BlueprintId == blueprint.Id)).Should().Be(0);
        // The favorite itself survives - only its entry for the deleted blueprint goes.
        (await verify.Favorites.AnyAsync(f => f.Id == favorite.Id)).Should().BeTrue();
    }
}
