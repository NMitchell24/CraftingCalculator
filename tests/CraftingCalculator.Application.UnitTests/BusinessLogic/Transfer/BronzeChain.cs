using CraftingCalculator.Domain.Enums;
using CraftingCalculator.Domain.Models.Transfer;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Transfer;

/// <summary>
/// Valheim's bronze chain as a snapshot, the one dataset every transfer test reads. Bronze is 2 Copper and
/// 1 Tin; a Bronze Axe is 4 Wood and 8 Bronze; one Bronze craft makes 20 Bronze Nails. Bronze is reached
/// through both the Axe and the Nails, and the Karve prep favorite holds both, so the chain has a shared
/// sub-recipe. Food is a category nothing is filed under, and Wood is uncategorized.
/// </summary>
internal static class BronzeChain
{
    public static readonly RecordKey Metals = new(RecordKind.Category, 1);
    public static readonly RecordKey Tools = new(RecordKind.Category, 2);
    public static readonly RecordKey Food = new(RecordKind.Category, 3);

    public static readonly RecordKey Copper = new(RecordKind.Component, 1);
    public static readonly RecordKey Tin = new(RecordKind.Component, 2);
    public static readonly RecordKey Wood = new(RecordKind.Component, 3);

    public static readonly RecordKey Bronze = new(RecordKind.Blueprint, 1);
    public static readonly RecordKey BronzeAxe = new(RecordKind.Blueprint, 2);
    public static readonly RecordKey BronzeNails = new(RecordKind.Blueprint, 3);

    public static readonly RecordKey AxeRun = new(RecordKind.Favorite, 1);
    public static readonly RecordKey KarvePrep = new(RecordKind.Favorite, 2);

    public static DatasetSnapshot Snapshot { get; } = new(
        "Valheim",
        [
            new SnapshotCategory(Metals.Id, "Metals", "Smelted in the furnace"),
            new SnapshotCategory(Tools.Id, "Tools", ""),
            new SnapshotCategory(Food.Id, "Food", "")
        ],
        [
            new SnapshotComponent(Copper.Id, "Copper", "Ore", 2, TimeSpan.FromSeconds(30), Metals.Id),
            new SnapshotComponent(Tin.Id, "Tin", "", 1, TimeSpan.Zero, Metals.Id),
            new SnapshotComponent(Wood.Id, "Wood", "", 0.5, TimeSpan.Zero, null)
        ],
        [
            new SnapshotBlueprint(Bronze.Id, "Bronze", "Alloy", 6, 1, TimeSpan.FromMinutes(1), Metals.Id,
                [new QuantityLink(Copper.Id, 2), new QuantityLink(Tin.Id, 1)], []),
            new SnapshotBlueprint(BronzeAxe.Id, "Bronze Axe", "", 40, 1, TimeSpan.Zero, Tools.Id,
                [new QuantityLink(Wood.Id, 4)], [new QuantityLink(Bronze.Id, 8)]),
            new SnapshotBlueprint(BronzeNails.Id, "Bronze Nails", "", 1, 20, TimeSpan.Zero, Metals.Id,
                [], [new QuantityLink(Bronze.Id, 1)])
        ],
        [
            new SnapshotFavorite(AxeRun.Id, "Bronze Axe run", [new QuantityLink(BronzeAxe.Id, 5)]),
            new SnapshotFavorite(KarvePrep.Id, "Karve prep",
                [new QuantityLink(BronzeNails.Id, 10), new QuantityLink(BronzeAxe.Id, 1)])
        ]);
}
