using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;
using static CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer.BronzeChain;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class SnapshotModelProcessorTests
{
    [Test]
    public void ToCategoryModel_CopiesTheCategory()
    {
        CategoryModel metals = SnapshotModelProcessor.ToCategoryModel(Snapshot, Metals.Id);

        (metals.Id, metals.Name, metals.Description).Should().Be((Metals.Id, "Metals", "Smelted in the furnace"));
    }

    [Test]
    public void ToComponentModel_CopiesEveryFieldAndItsCategory()
    {
        ComponentModel copper = SnapshotModelProcessor.ToComponentModel(Snapshot, Copper.Id);

        (copper.Name, copper.Description, copper.Cost, copper.ProductionTime)
            .Should().Be(("Copper", "Ore", 2.0, TimeSpan.FromSeconds(30)));
        copper.Category!.Name.Should().Be("Metals");
    }

    [Test]
    public void ToComponentModel_Uncategorized_HasNoCategory() =>
        SnapshotModelProcessor.ToComponentModel(Snapshot, Wood.Id).Category.Should().BeNull();

    [Test]
    public void ToBlueprintModel_BuildsTheWholeTree()
    {
        BlueprintModel axe = SnapshotModelProcessor.ToBlueprintModel(Snapshot, BronzeAxe.Id);

        axe.Category!.Name.Should().Be("Tools");
        axe.Components.ComponentList.Select(part => (part.Name, part.Quantity)).Should().Equal(("Wood", 4L));

        BlueprintQuantity bronze = axe.ChildBlueprints.BlueprintList.Should().ContainSingle().Subject;
        bronze.Quantity.Should().Be(8);
        bronze.Blueprint.Components.ComponentList.Select(part => (part.Name, part.Quantity))
            .Should().Equal(("Copper", 2L), ("Tin", 1L));
    }

    [Test]
    public void ToBlueprintModel_CopiesYieldAndProductionTime()
    {
        BlueprintModel nails = SnapshotModelProcessor.ToBlueprintModel(Snapshot, BronzeNails.Id);

        nails.Yield.Should().Be(20);
        SnapshotModelProcessor.ToBlueprintModel(Snapshot, Bronze.Id).ProductionTime.Should().Be(TimeSpan.FromMinutes(1));
    }

    [Test]
    public void ToBlueprintModel_ABlueprintNestedInsideItself_LeavesTheLoopOut()
    {
        DatasetSnapshot cyclic = new("Legacy", [], [],
        [
            new SnapshotBlueprint(1, "Bronze Plate", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(2, 1)]),
            new SnapshotBlueprint(2, "Bronze Nails", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(1, 1)])
        ], [], Datasettings.Default);

        BlueprintModel plate = SnapshotModelProcessor.ToBlueprintModel(cyclic, 1);

        BlueprintModel nails = plate.ChildBlueprints.BlueprintList.Should().ContainSingle().Subject.Blueprint;
        nails.ChildBlueprints.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void ToBlueprintModels_ABlueprintReachedTwice_IsBuiltOnce()
    {
        DatasetRecords records = new(Snapshot.Categories, Snapshot.Components, Snapshot.Blueprints);

        Dictionary<int, BlueprintModel> blueprints =
            SnapshotModelProcessor.ToBlueprintModels(records, [BronzeAxe.Id, BronzeNails.Id, Bronze.Id]);

        BlueprintModel bronze = blueprints[Bronze.Id];
        blueprints[BronzeAxe.Id].ChildBlueprints.BlueprintList.Single().Blueprint.Should().BeSameAs(bronze);
        blueprints[BronzeNails.Id].ChildBlueprints.BlueprintList.Single().Blueprint.Should().BeSameAs(bronze);
    }

    /// <summary>
    /// Which link of a loop is dropped depends on where the walk started, so a blueprint in a loop is built again for
    /// every root rather than shared: read as a root, Bronze Nails keeps the Bronze Plate it holds.
    /// </summary>
    [Test]
    public void ToBlueprintModels_ABlueprintInALoop_IsBuiltForEachRoot()
    {
        DatasetRecords cyclic = new([], [],
        [
            new SnapshotBlueprint(1, "Bronze Plate", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(2, 1)]),
            new SnapshotBlueprint(2, "Bronze Nails", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(1, 1)])
        ]);

        Dictionary<int, BlueprintModel> blueprints = SnapshotModelProcessor.ToBlueprintModels(cyclic, [1, 2]);

        BlueprintModel nestedNails = blueprints[1].ChildBlueprints.BlueprintList.Single().Blueprint;
        nestedNails.ChildBlueprints.BlueprintList.Should().BeEmpty();
        blueprints[2].Should().NotBeSameAs(nestedNails);
        blueprints[2].ChildBlueprints.BlueprintList.Single().Blueprint.Name.Should().Be("Bronze Plate");
    }

    [Test]
    public void ToBlueprintModels_ById_BuildsEachAskedForId_AndSkipsOnesTheRecordsDoNotHold()
    {
        DatasetRecords records = new(Snapshot.Categories, Snapshot.Components, Snapshot.Blueprints);

        Dictionary<int, BlueprintModel> blueprints =
            SnapshotModelProcessor.ToBlueprintModels(records, [BronzeAxe.Id, Bronze.Id, 999]);

        blueprints.Keys.Should().BeEquivalentTo([BronzeAxe.Id, Bronze.Id]);
        blueprints[BronzeAxe.Id].ChildBlueprints.BlueprintList.Single().Blueprint.Components.ComponentList
            .Select(part => (part.Name, part.Quantity)).Should().Equal(("Copper", 2L), ("Tin", 1L));
        blueprints[BronzeAxe.Id].ChildBlueprints.BlueprintList.Single().Blueprint.Should().BeSameAs(blueprints[Bronze.Id]);
    }

    [Test]
    public void ToFavoriteBlueprints_ReturnsEachBlueprintBuiltOutWithItsQuantity()
    {
        List<BlueprintQuantity> blueprints = SnapshotModelProcessor.ToFavoriteBlueprints(Snapshot, KarvePrep.Id);

        blueprints.Select(quantity => (quantity.Name, quantity.Quantity))
            .Should().Equal(("Bronze Nails", 10L), ("Bronze Axe", 1L));
        blueprints[0].Blueprint.ChildBlueprints.BlueprintList.Single().Name.Should().Be("Bronze");
    }
}
