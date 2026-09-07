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
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Blueprint plank = await _blueprintDAO.SaveAsync(new Blueprint { Name = "Plank" });

        Blueprint table = new() { Name = "Table", Value = 10 };
        table.Components.Add(wood, 2);
        table.ChildBlueprints.Add(plank, 4);

        Blueprint saved = await _blueprintDAO.SaveAsync(table);
        saved.Id.Should().BeGreaterThan(0);

        Blueprint? reloaded = await _blueprintDAO.GetByIdAsync(saved.Id);
        reloaded.Should().NotBeNull();
        reloaded!.Components.ComponentList.Should().ContainSingle(componentQuantity => componentQuantity.Component.Name == "Wood" && componentQuantity.Quantity == 2);
        reloaded.ChildBlueprints.BlueprintList.Should().ContainSingle(blueprintQuantity => blueprintQuantity.Blueprint.Name == "Plank" && blueprintQuantity.Quantity == 4);
    }

    [Test]
    public async Task SaveAsync_UpdatingExistingComponentQuantity_UpdatesInPlaceRatherThanDuplicating()
    {
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Blueprint table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        Blueprint saved = await _blueprintDAO.SaveAsync(table);

        Blueprint reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        ComponentQuantity existingComponent = reloaded.Components.ComponentList.Single();
        existingComponent.Quantity = 9;

        await _blueprintDAO.SaveAsync(reloaded);

        Blueprint reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().ContainSingle();
        reloadedAgain.Components.ComponentList[0].Quantity.Should().Be(9);
    }

    [Test]
    public async Task SaveAsync_RemovedComponent_DeletesItsRowRatherThanLeavingItOrphaned()
    {
        Component wood = await _componentDAO.SaveAsync(new Component { Name = "Wood", Cost = 1 });
        Blueprint table = new() { Name = "Table" };
        table.Components.Add(wood, 2);
        Blueprint saved = await _blueprintDAO.SaveAsync(table);

        Blueprint reloaded = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloaded.Components.Remove(wood, reloaded.Components.ComponentList[0].Quantity);

        await _blueprintDAO.SaveAsync(reloaded);

        Blueprint reloadedAgain = (await _blueprintDAO.GetByIdAsync(saved.Id))!;
        reloadedAgain.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public async Task DeleteAsync_RemovesTheBlueprint()
    {
        Blueprint saved = await _blueprintDAO.SaveAsync(new Blueprint { Name = "Table" });

        await _blueprintDAO.DeleteAsync(saved.Id);

        (await _blueprintDAO.GetByIdAsync(saved.Id)).Should().BeNull();
    }
}
