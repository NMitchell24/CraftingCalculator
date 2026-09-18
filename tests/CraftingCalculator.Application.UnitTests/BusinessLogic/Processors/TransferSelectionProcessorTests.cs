using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;
using static CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer.BronzeChain;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

/// <summary>
/// One test per rule in the export spec's "Selection behaviors" list, run against the Valheim bronze chain
/// (see <see cref="Transfer.BronzeChain"/>).
/// </summary>
[TestFixture]
public class TransferSelectionProcessorTests
{
    private static readonly DependencyGraph Graph = DependencyGraphProcessor.Build(Snapshot);

    private static HashSet<RecordKey> Everything() => [.. Graph.All];

    // Deselecting a category

    [Test]
    public void Deselect_ACategory_DeselectsEverythingFiledUnderItAndEverythingThatUsesThose()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), Metals);

        // The Axe is not filed under Metals, but it nests Bronze, which is.
        change.Removed.Should().BeEquivalentTo(
            [Metals, Copper, Tin, Bronze, BronzeNails, BronzeAxe, AxeRun, KarvePrep]);
        change.Added.Should().BeEmpty();
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Component] = 2,
            [RecordKind.Blueprint] = 3,
            [RecordKind.Favorite] = 2
        });
    }

    [Test]
    public void Deselect_ACategoryNothingUses_CascadesNothing()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), Food);

        change.Removed.Should().Equal(Food);
        change.CascadedByKind.Should().BeEmpty();
    }

    // Selecting a category

    [Test]
    public void Select_ACategory_SelectsOnlyTheCategory()
    {
        SelectionChange change = TransferSelectionProcessor.Select(Graph, new HashSet<RecordKey>(), Metals);

        change.Added.Should().Equal(Metals);
        change.CascadedByKind.Should().BeEmpty();
    }

    [Test]
    public void SelectUsersOf_ACategory_SelectsWhatIsFiledUnderItAndWhatThoseNeed()
    {
        HashSet<RecordKey> selected = [Metals];

        SelectionChange change = TransferSelectionProcessor.SelectUsersOf(Graph, selected, [Metals]);

        // The Axe only nests something filed under Metals, so it is not one of the category's own records.
        change.Added.Should().BeEquivalentTo([Copper, Tin, Bronze, BronzeNails]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Component] = 2,
            [RecordKind.Blueprint] = 2
        });
    }

    [Test]
    public void SelectUsersOf_AUserWithAnotherCategory_SelectsThatCategoryToo()
    {
        HashSet<RecordKey> selected = [Tools];

        SelectionChange change = TransferSelectionProcessor.SelectUsersOf(Graph, selected, [Tools]);

        change.Added.Should().BeEquivalentTo([BronzeAxe, Wood, Bronze, Copper, Tin, Metals]);
        change.CascadedByKind[RecordKind.Category].Should().Be(1);
    }

    [Test]
    public void SelectUsersOf_ACategoryNothingUses_SelectsNothing()
    {
        HashSet<RecordKey> selected = [Food];

        TransferSelectionProcessor.SelectUsersOf(Graph, selected, [Food]).Added.Should().BeEmpty();
    }

    [Test]
    public void SelectUsersOf_UsersAlreadySelected_SelectsNothing() =>
        TransferSelectionProcessor.SelectUsersOf(Graph, Everything(), [Metals]).Added.Should().BeEmpty();

    // Deselecting a component

    [Test]
    public void Deselect_AComponent_DeselectsEveryBlueprintThatUsesItAtAnyDepthAndTheirFavorites()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), Copper);

        change.Removed.Should().BeEquivalentTo([Copper, Bronze, BronzeAxe, BronzeNails, AxeRun, KarvePrep]);
        change.CascadedByKind.Should().NotContainKey(RecordKind.Component);
        change.CascadedByKind.Should().NotContainKey(RecordKind.Category);
    }

    [Test]
    public void Deselect_AComponentNothingSelectedUses_CascadesNothing()
    {
        HashSet<RecordKey> selected = [Metals, Copper];

        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, selected, Copper);

        change.Removed.Should().Equal(Copper);
        change.CascadedByKind.Should().BeEmpty();
    }

    // Selecting a component

    [Test]
    public void Select_AComponent_SelectsItsCategory()
    {
        SelectionChange change = TransferSelectionProcessor.Select(Graph, new HashSet<RecordKey>(), Copper);

        change.Added.Should().BeEquivalentTo([Copper, Metals]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int> { [RecordKind.Category] = 1 });
    }

    [Test]
    public void Select_AComponentWhoseCategoryIsSelected_AddsOnlyTheComponent()
    {
        HashSet<RecordKey> selected = [Metals];

        TransferSelectionProcessor.Select(Graph, selected, Copper).Added.Should().Equal(Copper);
    }

    // Deselecting a blueprint

    [Test]
    public void Deselect_ABlueprintOnlyFavoritesUse_CascadesOnlyFavorites()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), BronzeAxe);

        change.Removed.Should().BeEquivalentTo([BronzeAxe, AxeRun, KarvePrep]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int> { [RecordKind.Favorite] = 2 });
    }

    [Test]
    public void Deselect_ABlueprintOtherBlueprintsNest_DeselectsThemAndTheirFavorites()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), Bronze);

        change.Removed.Should().BeEquivalentTo([Bronze, BronzeAxe, BronzeNails, AxeRun, KarvePrep]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Blueprint] = 2,
            [RecordKind.Favorite] = 2
        });
    }

    [Test]
    public void Deselect_ABlueprint_LeavesWhatItNeedsSelected()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), BronzeAxe);

        change.Removed.Should().NotContain([Wood, Bronze, Tools]);
    }

    // Selecting a blueprint

    [Test]
    public void Select_ABlueprint_SelectsItsWholeChain()
    {
        SelectionChange change = TransferSelectionProcessor.Select(Graph, new HashSet<RecordKey>(), BronzeAxe);

        change.Added.Should().BeEquivalentTo([BronzeAxe, Tools, Wood, Bronze, Metals, Copper, Tin]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Category] = 2,
            [RecordKind.Component] = 3,
            [RecordKind.Blueprint] = 1
        });
    }

    [Test]
    public void Select_ABlueprint_SkipsWhatIsAlreadySelected()
    {
        HashSet<RecordKey> selected = [Metals, Copper, Tin, Bronze];

        TransferSelectionProcessor.Select(Graph, selected, BronzeAxe).Added.Should().BeEquivalentTo([BronzeAxe, Tools, Wood]);
    }

    // Deselecting a favorite

    [Test]
    public void Deselect_AFavorite_DeselectsOnlyTheFavorite()
    {
        SelectionChange change = TransferSelectionProcessor.Deselect(Graph, Everything(), AxeRun);

        change.Removed.Should().Equal(AxeRun);
        change.CascadedByKind.Should().BeEmpty();
    }

    // Selecting a favorite

    [Test]
    public void Select_AFavorite_SelectsEveryBlueprintComponentAndCategoryInItsChain()
    {
        SelectionChange change = TransferSelectionProcessor.Select(Graph, new HashSet<RecordKey>(), KarvePrep);

        change.Added.Should().BeEquivalentTo(
            [KarvePrep, BronzeNails, BronzeAxe, Bronze, Wood, Copper, Tin, Metals, Tools]);
    }

    [Test]
    public void Select_ASharedSubRecipe_IsCountedOnce()
    {
        // Bronze is reached through both the Nails and the Axe.
        SelectionChange change = TransferSelectionProcessor.Select(Graph, new HashSet<RecordKey>(), KarvePrep);

        change.CascadedByKind[RecordKind.Blueprint].Should().Be(3);
    }

    // Select all / deselect all

    [Test]
    public void SelectAll_Blueprints_SelectsEveryBlueprintAndWhatTheyNeed()
    {
        SelectionChange change = TransferSelectionProcessor.SelectAll(Graph, new HashSet<RecordKey>(), RecordKind.Blueprint);

        change.Added.Should().BeEquivalentTo([Bronze, BronzeAxe, BronzeNails, Metals, Tools, Copper, Tin, Wood]);

        // The blueprints are the records acted on, so only what they pulled in counts as cascaded.
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Category] = 2,
            [RecordKind.Component] = 3
        });
    }

    [Test]
    public void SelectAll_WithSomeAlreadySelected_AddsOnlyTheRest()
    {
        HashSet<RecordKey> selected = [Metals, Copper];

        TransferSelectionProcessor.SelectAll(Graph, selected, RecordKind.Component).Added.Should().BeEquivalentTo([Tin, Wood]);
    }

    [Test]
    public void DeselectAll_Components_DeselectsEveryBlueprintAndFavoriteThatUsesThem()
    {
        SelectionChange change = TransferSelectionProcessor.DeselectAll(Graph, Everything(), RecordKind.Component);

        change.Removed.Should().BeEquivalentTo([Copper, Tin, Wood, Bronze, BronzeAxe, BronzeNails, AxeRun, KarvePrep]);
        change.CascadedByKind.Should().BeEquivalentTo(new Dictionary<RecordKind, int>
        {
            [RecordKind.Blueprint] = 3,
            [RecordKind.Favorite] = 2
        });
    }

    [Test]
    public void DeselectAll_Favorites_CascadesNothing() =>
        TransferSelectionProcessor.DeselectAll(Graph, Everything(), RecordKind.Favorite).CascadedByKind.Should().BeEmpty();

    // State and closure

    [Test]
    public void StateOf_ReportsNoneSomeAndAll()
    {
        HashSet<RecordKey> selected = [Metals, Copper];

        TransferSelectionProcessor.StateOf(Graph, selected, RecordKind.Blueprint).Should().Be(SelectionState.None);
        TransferSelectionProcessor.StateOf(Graph, selected, RecordKind.Component).Should().Be(SelectionState.Some);
        TransferSelectionProcessor.StateOf(Graph, Everything(), RecordKind.Component).Should().Be(SelectionState.All);
    }

    [Test]
    public void StateOf_AKindWithNoRecords_IsNone()
    {
        DependencyGraph empty = DependencyGraphProcessor.Build(new DatasetSnapshot("Empty", [], [], [], [], Datasettings.Default));

        TransferSelectionProcessor.StateOf(empty, new HashSet<RecordKey>(), RecordKind.Favorite).Should().Be(SelectionState.None);
    }

    [TestCase(0, 3, SelectionState.None)]
    [TestCase(2, 3, SelectionState.Some)]
    [TestCase(3, 3, SelectionState.All)]
    [TestCase(0, 0, SelectionState.None)]
    public void StateOf_Counts_ReportsNoneSomeAndAll(int selected, int count, SelectionState expected)
    {
        TransferSelectionProcessor.StateOf(selected, count).Should().Be(expected);
    }

    [Test]
    public void IsClosed_EverySelectionTheRulesProduce_IsClosed()
    {
        HashSet<RecordKey> selected = Everything();
        selected.ExceptWith(TransferSelectionProcessor.Deselect(Graph, selected, Tin).Removed);

        TransferSelectionProcessor.IsClosed(Graph, selected).Should().BeTrue();
    }

    [Test]
    public void IsClosed_ABlueprintWithoutItsComponent_IsNotClosed()
    {
        HashSet<RecordKey> selected = [Metals, Tin, Bronze];

        TransferSelectionProcessor.IsClosed(Graph, selected).Should().BeFalse();
    }

    [Test]
    public void Select_OnCyclicData_Terminates()
    {
        DatasetSnapshot cyclic = new("Legacy", [], [],
        [
            new SnapshotBlueprint(1, "Bronze Plate", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(2, 1)]),
            new SnapshotBlueprint(2, "Bronze Nails", "", 0, 1, TimeSpan.Zero, null, [], [new QuantityLink(1, 1)])
        ], [], Datasettings.Default);
        DependencyGraph graph = DependencyGraphProcessor.Build(cyclic);

        TransferSelectionProcessor.Select(graph, new HashSet<RecordKey>(), new RecordKey(RecordKind.Blueprint, 1))
            .Added.Should().HaveCount(2);
    }

    [Test]
    public void CountSelected_CountsOnlyTheKindAsked()
    {
        HashSet<RecordKey> selected = [Metals, Copper, Tin, Bronze];

        TransferSelectionProcessor.CountSelected(Graph, selected, RecordKind.Component).Should().Be(2);
        TransferSelectionProcessor.CountSelected(Graph, selected, RecordKind.Favorite).Should().Be(0);
    }

    [Test]
    public void Extract_KeepsTheSelectedRecordsWithTheirIdsAndLinks()
    {
        HashSet<RecordKey> selected = [Metals, Copper, Tin, Bronze];

        DatasetSnapshot extract = TransferSelectionProcessor.Extract(Snapshot, selected);

        extract.Should().BeEquivalentTo(new DatasetSnapshot(
            "Valheim",
            [Snapshot.Categories[0]],
            [Snapshot.Components[0], Snapshot.Components[1]],
            [Snapshot.Blueprints[0]],
            [],
            Snapshot.Settings));
    }

    [Test]
    public void Extract_ASelectionMissingADependency_Throws()
    {
        Action act = () => TransferSelectionProcessor.Extract(Snapshot, new HashSet<RecordKey> { Bronze });

        act.Should().Throw<InvalidOperationException>();
    }
}
