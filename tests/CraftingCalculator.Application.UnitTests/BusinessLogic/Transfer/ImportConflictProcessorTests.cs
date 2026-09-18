using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Transfer;
using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models;
using CraftingCalculator.Domain.Models.Transfer;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

[TestFixture]
public class ImportConflictProcessorTests
{
    private static DatasetSnapshot Snapshot(
        IReadOnlyList<SnapshotCategory>? categories = null,
        IReadOnlyList<SnapshotComponent>? components = null,
        IReadOnlyList<SnapshotBlueprint>? blueprints = null,
        IReadOnlyList<SnapshotFavorite>? favorites = null) =>
        new("Valheim", categories ?? [], components ?? [], blueprints ?? [], favorites ?? [], Datasettings.Default);

    private static SnapshotComponent Component(int id, string name) => new(id, name, "", 0, TimeSpan.Zero, null);

    private static SnapshotBlueprint Blueprint(int id, string name, params int[] children) =>
        new(id, name, "", 0, 1, TimeSpan.Zero, null, [], [.. children.Select(child => new QuantityLink(child, 1))]);

    private static RecordKey BlueprintKey(int id) => new(RecordKind.Blueprint, id);

    [Test]
    public void Find_TheBronzeChainAgainstItself_ConflictsOnEveryRecord()
    {
        IReadOnlyList<ImportConflict> conflicts = ImportConflictProcessor.Find(BronzeChain.Snapshot, BronzeChain.Snapshot);

        conflicts.Select(conflict => (conflict.Kind, conflict.IncomingId, conflict.ExistingId)).Should().Equal(
            (RecordKind.Category, 1, 1), (RecordKind.Category, 2, 2), (RecordKind.Category, 3, 3),
            (RecordKind.Component, 1, 1), (RecordKind.Component, 2, 2), (RecordKind.Component, 3, 3),
            (RecordKind.Blueprint, 1, 1), (RecordKind.Blueprint, 2, 2), (RecordKind.Blueprint, 3, 3),
            (RecordKind.Favorite, 1, 1), (RecordKind.Favorite, 2, 2));
    }

    [Test]
    public void Find_MatchesNamesIgnoringCaseAndSurroundingSpaces()
    {
        DatasetSnapshot incoming = Snapshot(components: [Component(1, "  copper ")]);
        DatasetSnapshot current = Snapshot(components: [Component(7, "Copper")]);

        ImportConflictProcessor.Find(incoming, current).Should().Equal(new ImportConflict(RecordKind.Component, 1, 7, "  copper "));
    }

    [Test]
    public void Find_OnlyMatchesARecordOfTheSameKind()
    {
        DatasetSnapshot incoming = Snapshot(blueprints: [Blueprint(1, "Bronze")]);
        DatasetSnapshot current = Snapshot(components: [Component(1, "Bronze")]);

        ImportConflictProcessor.Find(incoming, current).Should().BeEmpty();
    }

    [Test]
    public void Find_WhenTheDatasetHasTwoRecordsWithTheName_MatchesTheLowestId()
    {
        DatasetSnapshot incoming = Snapshot(components: [Component(1, "Wood")]);
        DatasetSnapshot current = Snapshot(components: [Component(9, "Wood"), Component(4, "wood")]);

        ImportConflictProcessor.Find(incoming, current).Single().ExistingId.Should().Be(4);
    }

    [Test]
    public void Find_WhenTheFileHasTwoRecordsWithTheName_MatchesOnlyTheFirst()
    {
        DatasetSnapshot incoming = Snapshot(components: [Component(1, "Wood"), Component(2, "Wood")]);
        DatasetSnapshot current = Snapshot(components: [Component(5, "Wood")]);

        ImportConflictProcessor.Find(incoming, current).Should().Equal(new ImportConflict(RecordKind.Component, 1, 5, "Wood"));
    }

    [Test]
    public void Find_NoSharedNames_FindsNoConflicts() =>
        ImportConflictProcessor.Find(Snapshot(components: [Component(1, "Sulfur")]), BronzeChain.Snapshot).Should().BeEmpty();

    // Keep Mine and Replace Mine can't create a loop when neither side has one. Keep Mine adds records that point
    // into the dataset, but nothing already in the dataset points back at them. Replace Mine overwrites every
    // conflict, so the records the file writes only point at other records the file wrote, and those links are the
    // file's own, which have no loop. (That rests on each dataset record being landed on by at most one incoming
    // record, which Find guarantees.) Only a mix can: a kept record pointing one way and a replaced one pointing
    // back, as in the Bronze Nails case below.

    private static readonly DatasetSnapshot Mine = Snapshot(blueprints: [Blueprint(1, "Bronze Plate"), Blueprint(2, "Bronze Nails", 1)]);
    private static readonly DatasetSnapshot Theirs = Snapshot(blueprints: [Blueprint(1, "Bronze Plate", 2), Blueprint(2, "Bronze Nails")]);

    [Test]
    public void FindNewCycles_KeepingEveryConflict_FindsNone()
    {
        MergePlan plan = new(Theirs, ImportConflictProcessor.Find(Theirs, Mine), new HashSet<RecordKey>());

        ImportConflictProcessor.FindNewCycles(plan, Mine).Should().BeEmpty();
    }

    [Test]
    public void FindNewCycles_ReplacingEveryConflict_FindsNone()
    {
        MergePlan plan = new(Theirs, ImportConflictProcessor.Find(Theirs, Mine), new HashSet<RecordKey> { BlueprintKey(1), BlueprintKey(2) });

        ImportConflictProcessor.FindNewCycles(plan, Mine).Should().BeEmpty();
    }

    [Test]
    public void FindNewCycles_KeepingOneAndReplacingTheOther_NamesTheLoopTheyMake()
    {
        // Mine has the Nails using the Plate; the file has the Plate using the Nails. Keeping my Nails and taking the
        // file's Plate leaves each inside the other.
        MergePlan plan = new(Theirs, ImportConflictProcessor.Find(Theirs, Mine), new HashSet<RecordKey> { BlueprintKey(1) });

        ImportConflictProcessor.FindNewCycles(plan, Mine).Should().Equal("Bronze Nails", "Bronze Plate");
    }

    [Test]
    public void FindNewCycles_AnAddedBlueprintNestingAKeptOne_FindsNone()
    {
        DatasetSnapshot incoming = Snapshot(blueprints: [Blueprint(1, "Bronze Plate"), Blueprint(2, "Bronze Rivets", 1)]);
        MergePlan plan = new(incoming, ImportConflictProcessor.Find(incoming, Mine), new HashSet<RecordKey>());

        ImportConflictProcessor.FindNewCycles(plan, Mine).Should().BeEmpty();
    }

    [Test]
    public void FindNewCycles_ALoopAlreadyInTheDataset_IsNotReported()
    {
        DatasetSnapshot legacy = Snapshot(blueprints: [Blueprint(1, "Ouroboros", 1)]);
        DatasetSnapshot incoming = Snapshot(blueprints: [Blueprint(1, "Bronze")]);
        MergePlan plan = new(incoming, ImportConflictProcessor.Find(incoming, legacy), new HashSet<RecordKey>());

        ImportConflictProcessor.FindNewCycles(plan, legacy).Should().BeEmpty();
    }
}
