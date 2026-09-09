using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Infrastructure.DAO.Impl;

namespace CraftingCalculator.Infrastructure.UnitTests.DAO.Impl;

[TestFixture]
public class BlueprintDAOTests
{
    private SqliteTestFixture _fixture = null!;
    private BlueprintDAO _blueprintDAO = null!;
    private ComponentDAO _componentDAO = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new SqliteTestFixture();
        _blueprintDAO = new BlueprintDAO(_fixture.Factory);
        _componentDAO = new ComponentDAO(_fixture.Factory);
    }

    [TearDown]
    public void TearDown() => _fixture.Dispose();

    [Test]
    public async Task SaveAsync_NewBlueprintWithComponentAndChildBlueprint_PersistsTheFullGraph()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel plank = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        BlueprintModel table = new() { Name = "Table", Value = 10 };
        table.Components.Add(wood, 2);
        table.ChildBlueprints.Add(plank, 4);

        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);
        saved.Id.Should().BeGreaterThan(0);

        BlueprintModel? reloaded = await _blueprintDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded.Components.ComponentList.Should().ContainSingle(componentQuantity => componentQuantity.Component.Name == "Wood" && componentQuantity.Quantity == 2);
        reloaded.ChildBlueprints.BlueprintList.Should().ContainSingle(blueprintQuantity => blueprintQuantity.Blueprint.Name == "Plank" && blueprintQuantity.Quantity == 4);
    }

    [Test]
    public async Task SaveAsync_Yield_RoundTripsThroughTheDatabase()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Bracket", Yield = 4 });

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.Yield.Should().Be(4);

        reloaded.Yield = 7;
        await _blueprintDAO.SaveAsync(reloaded);

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.Yield.Should().Be(7);
    }

    /// <summary>
    /// Production time is stored as ticks through a value converter, so this covers the whole round trip
    /// rather than just the property: a tenth of a second has to survive the conversion in both
    /// directions, and on a component as well as a blueprint.
    /// </summary>
    [Test]
    public async Task SaveAsync_ProductionTime_RoundTripsThroughTheDatabase()
    {
        ComponentModel potato = await _componentDAO.SaveAsync(
            new ComponentModel { Name = "Potato", ProductionTime = TimeSpan.FromMinutes(5) });

        BlueprintModel soup = new() { Name = "Soup", ProductionTime = TimeSpan.FromSeconds(5.5) };
        soup.Components.Add(potato, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(soup);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.ProductionTime.Should().Be(TimeSpan.FromSeconds(5.5));
        reloaded.Components.ComponentList[0].Component.ProductionTime.Should().Be(TimeSpan.FromMinutes(5));

        reloaded.ProductionTime = TimeSpan.FromHours(2);
        await _blueprintDAO.SaveAsync(reloaded);

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.ProductionTime.Should().Be(TimeSpan.FromHours(2));
    }

    [Test]
    public async Task SaveAsync_WithoutAnExplicitProductionTime_DefaultsToInstant()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.ProductionTime.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public async Task SaveAsync_BlueprintWithoutAnExplicitYield_DefaultsToOne()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Plank" });

        (await _blueprintDAO.GetByIdAsync(saved.Id))!.Yield.Should().Be(1);
    }

    [Test]
    public async Task SaveAsync_UpdatingExistingComponentQuantity_UpdatesInPlaceRatherThanDuplicating()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        ComponentQuantity existingComponent = reloaded.Components.ComponentList.Single();
        existingComponent.Quantity = 9;

        await _blueprintDAO.SaveAsync(reloaded);

        BlueprintModel reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().ContainSingle();
        reloadedAgain.Components.ComponentList[0].Quantity.Should().Be(9);
    }

    [Test]
    public async Task SaveAsync_RemovedComponent_DeletesItsRowRatherThanLeavingItOrphaned()
    {
        ComponentModel wood = await _componentDAO.SaveAsync(new ComponentModel { Name = "Wood", Cost = 1 });
        BlueprintModel table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        BlueprintModel saved = await _blueprintDAO.SaveAsync(table);

        BlueprintModel reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.Components.Remove(wood, reloaded.Components.ComponentList[0].Quantity);

        await _blueprintDAO.SaveAsync(reloaded);

        BlueprintModel reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_RemovesTheBlueprint()
    {
        BlueprintModel saved = await _blueprintDAO.SaveAsync(new BlueprintModel { Name = "Table" });

        await _blueprintDAO.DeleteAsync(saved.Id);

        (await _blueprintDAO.GetByIdAsync(saved.Id)).Should().BeNull();
    }
}
