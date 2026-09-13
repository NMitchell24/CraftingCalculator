using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;
using static CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer.BronzeChain;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

/// <summary>
/// Pins every edge <see cref="DependencyGraphProcessor.Build"/> draws. A new link between records belongs in
/// Build and in a test here; the selection rules never learn about it.
/// </summary>
[TestFixture]
public class DependencyGraphProcessorTests
{
    private static readonly DependencyGraph Graph = DependencyGraphProcessor.Build(Snapshot);

    [Test]
    public void Build_ListsEveryRecordOnce() =>
        Graph.All.Should().BeEquivalentTo(
            [Metals, Tools, Food, Copper, Tin, Wood, Bronze, BronzeAxe, BronzeNails, AxeRun, KarvePrep]);

    [Test]
    public void Build_ACategoryDependsOnNothing() => Graph.DependsOn[Metals].Should().BeEmpty();

    [Test]
    public void Build_AComponentDependsOnItsCategory() => Graph.DependsOn[Copper].Should().Equal(Metals);

    [Test]
    public void Build_AnUncategorizedComponentDependsOnNothing() => Graph.DependsOn[Wood].Should().BeEmpty();

    [Test]
    public void Build_ABlueprintDependsOnItsCategoryComponentsAndNestedBlueprints()
    {
        Graph.DependsOn[Bronze].Should().BeEquivalentTo([Metals, Copper, Tin]);
        Graph.DependsOn[BronzeAxe].Should().BeEquivalentTo([Tools, Wood, Bronze]);
    }

    [Test]
    public void Build_AFavoriteDependsOnItsBlueprints() =>
        Graph.DependsOn[KarvePrep].Should().BeEquivalentTo([BronzeNails, BronzeAxe]);

    [Test]
    public void Build_UsedByIsDependsOnReversed()
    {
        Graph.UsedBy[Metals].Should().BeEquivalentTo([Copper, Tin, Bronze, BronzeNails]);
        Graph.UsedBy[Bronze].Should().BeEquivalentTo([BronzeAxe, BronzeNails]);
        Graph.UsedBy[BronzeAxe].Should().BeEquivalentTo([AxeRun, KarvePrep]);
        Graph.UsedBy[Food].Should().BeEmpty();
        Graph.UsedBy[AxeRun].Should().BeEmpty();
    }

    [Test]
    public void Build_TheSameComponentListedTwice_IsOneEdge()
    {
        DatasetSnapshot snapshot = SnapshotOf(
            new SnapshotBlueprint(1, "Bronze", "", 6, 1, TimeSpan.Zero, null,
                [new QuantityLink(1, 1), new QuantityLink(1, 1)], []),
            components: [new SnapshotComponent(1, "Copper", "", 2, TimeSpan.Zero, null)]);

        DependencyGraph graph = DependencyGraphProcessor.Build(snapshot);

        graph.UsedBy[new RecordKey(RecordKind.Component, 1)].Should().ContainSingle();
    }

    [Test]
    public void FindCycles_TheBronzeChain_FindsNone() =>
        DependencyGraphProcessor.FindCycles(Graph).Should().BeEmpty();

    [Test]
    public void FindCycles_ABlueprintNestingItself_FindsIt()
    {
        DependencyGraph graph = DependencyGraphProcessor.Build(SnapshotOf(Blueprint(1, "Bronze Plate", 1)));

        DependencyGraphProcessor.FindCycles(graph).Should().Equal(BlueprintKey(1));
    }

    [Test]
    public void FindCycles_TwoBlueprintsNestingEachOther_FindsBoth()
    {
        DependencyGraph graph = DependencyGraphProcessor.Build(
            SnapshotOf(Blueprint(1, "Bronze Plate", 2), Blueprint(2, "Bronze Nails", 1)));

        DependencyGraphProcessor.FindCycles(graph).Should().BeEquivalentTo([BlueprintKey(1), BlueprintKey(2)]);
    }

    [Test]
    public void FindCycles_ADeepLoop_FindsOnlyTheBlueprintsOnIt()
    {
        // Karve nests the loop without being part of it; the loop runs Plate -> Nails -> Rivets -> Hull -> Plate.
        DependencyGraph graph = DependencyGraphProcessor.Build(SnapshotOf(
            Blueprint(1, "Karve", 2),
            Blueprint(2, "Bronze Plate", 3),
            Blueprint(3, "Bronze Nails", 4),
            Blueprint(4, "Rivets", 5),
            Blueprint(5, "Hull", 2)));

        DependencyGraphProcessor.FindCycles(graph)
            .Should().BeEquivalentTo([BlueprintKey(2), BlueprintKey(3), BlueprintKey(4), BlueprintKey(5)]);
    }

    [Test]
    public void FindCycles_ABlueprintBetweenTwoLoops_IsNotReported()
    {
        // 1 <-> 2 is one loop, 4 <-> 5 another; 3 is nested by the first and nests the second, but nothing
        // nests 3 back.
        DependencyGraph graph = DependencyGraphProcessor.Build(SnapshotOf(
            Blueprint(1, "A", 2),
            Blueprint(2, "B", 1, 3),
            Blueprint(3, "Between", 4),
            Blueprint(4, "C", 5),
            Blueprint(5, "D", 4)));

        DependencyGraphProcessor.FindCycles(graph)
            .Should().BeEquivalentTo([BlueprintKey(1), BlueprintKey(2), BlueprintKey(4), BlueprintKey(5)]);
    }

    [Test]
    public void FindCycles_AChainDeeperThanTheCallStack_Completes()
    {
        const int depth = 100_000;
        SnapshotBlueprint[] chain =
            [.. Enumerable.Range(1, depth).Select(id => Blueprint(id, $"Step {id}", id == depth ? [] : [id + 1]))];

        DependencyGraph graph = DependencyGraphProcessor.Build(SnapshotOf(chain));

        DependencyGraphProcessor.FindCycles(graph).Should().BeEmpty();
    }

    private static RecordKey BlueprintKey(int id) => new(RecordKind.Blueprint, id);

    private static SnapshotBlueprint Blueprint(int id, string name, params int[] nested) =>
        new(id, name, "", 0, 1, TimeSpan.Zero, null, [], [.. nested.Select(child => new QuantityLink(child, 1))]);

    private static DatasetSnapshot SnapshotOf(params SnapshotBlueprint[] blueprints) =>
        SnapshotOf(blueprints, components: []);

    private static DatasetSnapshot SnapshotOf(SnapshotBlueprint blueprint, SnapshotComponent[] components) =>
        SnapshotOf([blueprint], components);

    private static DatasetSnapshot SnapshotOf(SnapshotBlueprint[] blueprints, SnapshotComponent[] components) =>
        new("Test", [], components, blueprints, []);
}
