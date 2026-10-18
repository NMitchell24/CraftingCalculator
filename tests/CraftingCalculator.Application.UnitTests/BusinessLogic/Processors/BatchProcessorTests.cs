using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BatchProcessorTests
{
    // Maps and batches match records on id, so each name gets an id of its own and a name used twice is one record.
    private static readonly Dictionary<string, int> Ids = [];

    private static int IdOf(string name)
    {
        if (Ids.TryGetValue(name, out int id))
        {
            return id;
        }

        id = Ids.Count + 1;
        Ids[name] = id;
        return id;
    }

    private static readonly Datasettings NoYield = new(UseYield: false);

    private static readonly Datasettings NotShared = new(UseSharedSurplus: false);

    private static ComponentModel NewComponent(string name, double cost = 0) =>
        new() { Id = IdOf(name), Name = name, Cost = cost };

    private static BlueprintModel NewBlueprint(string name, double value = 0, long yield = 1) =>
        new() { Id = IdOf(name), Name = name, Value = value, Yield = yield };

    private static Batch BatchOf(IReadOnlyList<BlueprintQuantity> entries, params BlueprintModel[] alreadyCrafted) =>
        new(entries, alreadyCrafted.Select(blueprint => blueprint.Id).ToHashSet());

    private static BatchTotals TotalsOf(BlueprintModel blueprint, long quantity, Datasettings? settings = null) =>
        BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, quantity)]), settings ?? Datasettings.Default);

    private static BlueprintNode TreeOf(BlueprintModel blueprint, long quantity, Datasettings? settings = null) =>
        TotalsOf(blueprint, quantity, settings).Roots.Should().ContainSingle().Subject;

    [Test]
    public void CalculateTotals_SingleBlueprint_ReturnsCostAndValue()
    {
        BlueprintModel blueprint = NewBlueprint("Widget", 10);
        blueprint.Components.Add(NewComponent("Screw", 0.5), 2);

        BlueprintQuantity batchEntry = new(blueprint, 3);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([batchEntry]), Datasettings.Default);

        totals.TotalCost.Should().Be(3.0); // 0.5 x2 Screws x3 Widgets
        totals.TotalValue.Should().Be(30); // 10 x3 Widgets
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_NestedBlueprint_ScalesChildComponentsByBatchAndParentQuantity()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw", 1), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        BlueprintQuantity batchEntry = new(parent, 2);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([batchEntry]), Datasettings.Default);

        totals.TotalCost.Should().Be(12); // 1 x3 Screws x2 Brackets x2 Frames
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(12);
    }

    [Test]
    public void CalculateTotals_TwoBlueprintsSharingComponent_MergesMaterialsAndSumsValue()
    {
        BlueprintModel left = NewBlueprint("Left Arm", 5);
        left.Components.Add(NewComponent("Bolt", 1), 1);

        BlueprintModel right = NewBlueprint("Right Arm", 7);
        right.Components.Add(NewComponent("Bolt", 1), 1);

        BlueprintQuantity leftEntry = new(left, 1);
        BlueprintQuantity rightEntry = new(right, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([leftEntry, rightEntry]), Datasettings.Default);

        totals.TotalCost.Should().Be(2);
        totals.TotalValue.Should().Be(12);
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_TwoComponentsSharingAName_CostsEachAtItsOwnPrice()
    {
        BlueprintModel blueprint = NewBlueprint("Hatchet", 50);
        blueprint.Components.Add(new ComponentModel { Id = 100, Name = "Metal Fragments", Cost = 1 }, 75);
        blueprint.Components.Add(new ComponentModel { Id = 101, Name = "Metal Fragments", Cost = 2 }, 25);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 1)]), Datasettings.Default);

        totals.Materials.ComponentList.Should().HaveCount(2);
        totals.TotalCost.Should().Be(125);
    }

    [Test]
    public void CalculateTotals_ZeroCostComponent_ContributesNothingToCost()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Free Sample"), 5);

        BlueprintQuantity batchEntry = new(blueprint, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([batchEntry]), Datasettings.Default);

        totals.TotalCost.Should().Be(0);
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(5);
    }

    [Test]
    public void CalculateTotals_Yield_ChargesOnlyForTheCraftsNeeded()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 3);

        BlueprintQuantity batchEntry = new(blueprint, 4);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([batchEntry]), Datasettings.Default);

        totals.TotalCost.Should().Be(6); // 3 Screws x2 crafts, not x4 Brackets
        totals.Materials.ComponentList[0].Quantity.Should().Be(6);
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_YieldNotUsed_ChargesForEveryItemAndHasNoSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", 5, yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 3);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 3)]), new Datasettings(UseYield: false));

        totals.TotalCost.Should().Be(9); // 3 Screws x3 crafts, one per Bracket
        totals.TotalValue.Should().Be(15);
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_CostsNotUsed_CostsNothingAndKeepsTheValue()
    {
        BlueprintModel blueprint = NewBlueprint("Bronze", 10);
        blueprint.Components.Add(NewComponent("Copper", 2), 2);
        blueprint.Components.Add(NewComponent("Tin", 3), 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 5)]), new Datasettings(UseCosts: false));

        totals.TotalCost.Should().Be(0);
        totals.TotalValue.Should().Be(50);
        totals.Materials.ComponentList.Select(componentQuantity => componentQuantity.Quantity).Should().BeEquivalentTo([10L, 5L]);
    }

    [Test]
    public void CalculateTotals_ValuesNotUsed_IsWorthNothingAndKeepsTheCost()
    {
        BlueprintModel blueprint = NewBlueprint("Bronze", 10);
        blueprint.Components.Add(NewComponent("Copper", 2), 2);
        blueprint.Components.Add(NewComponent("Tin", 3), 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 5)]), new Datasettings(UseValues: false));

        totals.TotalCost.Should().Be(35); // 10 Copper x2 + 5 Tin x3
        totals.TotalValue.Should().Be(0);
    }

    [Test]
    public void CalculateTotals_NeitherCostsNorValuesUsed_StillCountsTheMaterials()
    {
        BlueprintModel blueprint = NewBlueprint("Bronze", 10);
        blueprint.Components.Add(NewComponent("Copper", 2), 2);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(blueprint, 5)]), new Datasettings(UseCosts: false, UseValues: false));

        totals.TotalCost.Should().Be(0);
        totals.TotalValue.Should().Be(0);
        totals.Materials.ComponentList.Should().ContainSingle().Which.Quantity.Should().Be(10);
    }

    [Test]
    public void CalculateTotals_SurplusNotShared_RoundsEachPositionIndependently()
    {
        // Each parent needs one Bracket, so each runs its own Bracket craft rather than sharing one
        // between them - and each craft leaves a spare.
        BlueprintModel bracket = NewBlueprint("Bracket", 5, yield: 2);
        bracket.Components.Add(NewComponent("Screw", 1), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 1);

        BlueprintModel handle = NewBlueprint("Handle");
        handle.ChildBlueprints.Add(bracket, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 1), new BlueprintQuantity(handle, 1)]), NotShared);

        totals.TotalCost.Should().Be(6); // two Bracket crafts, 3 Screws each
        BlueprintQuantity spare = totals.Surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Bracket");
        spare.Quantity.Should().Be(2);
        spare.TotalValue.Should().Be(10);
        totals.Roots.Select(root => root.Children.Single().FromSurplus).Should().Equal(0L, 0L);
    }

    [Test]
    public void CalculateTotals_TwoBlueprintsNestingTheSameYieldingChild_ShareOneCraft()
    {
        BlueprintModel bracket = NewBlueprint("Bracket", 5, yield: 2);
        bracket.Components.Add(NewComponent("Screw", 1), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 1);

        BlueprintModel handle = NewBlueprint("Handle");
        handle.ChildBlueprints.Add(bracket, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 1), new BlueprintQuantity(handle, 1)]), Datasettings.Default);

        totals.TotalCost.Should().Be(3); // one Bracket craft makes both Brackets
        totals.Crafts.Should().Be(3); // Frame, Handle, and one Bracket
        totals.Surplus.BlueprintList.Should().BeEmpty();
        totals.SurplusValue.Should().Be(0);

        BlueprintNode made = totals.Roots[0].Children.Single();
        made.Should().Match<BlueprintNode>(node => node.Crafts == 1 && node.Surplus == 1 && node.FromSurplus == 0);
        BlueprintNode used = totals.Roots[1].Children.Single();
        used.Should().Match<BlueprintNode>(node =>
            node.Quantity == 1 && node.Crafts == 0 && node.FromSurplus == 1 && node.Children.Count == 0);
    }

    [Test]
    public void CalculateTotals_SharedSurplus_SiblingsUnderOneParentShareTheirChildsCraft()
    {
        // A needs B, C and D; C needs 1 E and D needs 2 E, and one craft of E makes 3.
        BlueprintModel e = NewBlueprint("E", yield: 3);
        e.Components.Add(NewComponent("E Ore"), 1);

        BlueprintModel c = NewBlueprint("C");
        c.ChildBlueprints.Add(e, 1);

        BlueprintModel d = NewBlueprint("D");
        d.ChildBlueprints.Add(e, 2);

        BlueprintModel a = NewBlueprint("A");
        a.ChildBlueprints.Add(NewBlueprint("B"), 1);
        a.ChildBlueprints.Add(c, 1);
        a.ChildBlueprints.Add(d, 1);

        BatchTotals totals = TotalsOf(a, 1);

        totals.Crafts.Should().Be(5); // A, B, C, D, and a single E
        totals.Materials.ComponentList.Should().ContainSingle().Which.Quantity.Should().Be(1);
        totals.Surplus.BlueprintList.Should().BeEmpty();
        BlueprintNode eUnderD = totals.Roots.Single().Children[2].Children.Single();
        eUnderD.Should().Match<BlueprintNode>(node =>
            node.Quantity == 2 && node.FromSurplus == 2 && node.Crafts == 0 && node.Children.Count == 0);
    }

    [Test]
    public void CalculateTotals_SharedSurplus_TakesTheFewestCraftsAcrossTheBatch()
    {
        // Minecraft: 1 log makes 4 planks, 2 planks make 4 sticks. A wooden pickaxe is 3 planks and 2 sticks,
        // a wooden sword 2 planks and 1 stick.
        BlueprintModel planks = NewBlueprint("Planks", yield: 4);
        planks.Components.Add(NewComponent("Log"), 1);

        BlueprintModel sticks = NewBlueprint("Stick", yield: 4);
        sticks.ChildBlueprints.Add(planks, 2);

        BlueprintModel pickaxe = NewBlueprint("Wooden Pickaxe");
        pickaxe.ChildBlueprints.Add(planks, 3);
        pickaxe.ChildBlueprints.Add(sticks, 2);

        BlueprintModel sword = NewBlueprint("Wooden Sword");
        sword.ChildBlueprints.Add(planks, 2);
        sword.ChildBlueprints.Add(sticks, 1);

        BlueprintQuantity[] batch = [new(pickaxe, 1), new(sword, 1)];

        BatchTotals shared = BatchProcessor.CalculateTotals(BatchOf(batch), Datasettings.Default);
        BatchTotals notShared = BatchProcessor.CalculateTotals(BatchOf(batch), NotShared);

        // 7 planks in two crafts and 3 sticks in one, rather than four Planks crafts and two Stick crafts.
        shared.Crafts.Should().Be(5);
        shared.Materials.ComponentList.Single().Quantity.Should().Be(2);
        shared.Surplus.BlueprintList.Select(spare => (spare.Name, spare.Quantity))
            .Should().BeEquivalentTo([("Planks", 1L), ("Stick", 1L)]);
        notShared.Crafts.Should().Be(8);
        notShared.Materials.ComponentList.Single().Quantity.Should().Be(4);

        // The sword's planks and stick are all leftovers from the pickaxe.
        shared.Roots[1].Children.Select(node => (node.Crafts, node.FromSurplus)).Should().Equal((0L, 2L), (0L, 1L));
    }

    [Test]
    public void CalculateTotals_SharedSurplus_ShortOfTheNeedCraftsOnlyTheRest()
    {
        BlueprintModel bronze = NewBlueprint("Bronze", yield: 3);
        bronze.Components.Add(NewComponent("Copper"), 2);

        BlueprintModel nails = NewBlueprint("Bronze Nails");
        nails.ChildBlueprints.Add(bronze, 1);

        BlueprintModel axe = NewBlueprint("Bronze Axe");
        axe.ChildBlueprints.Add(bronze, 3);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(nails, 1), new BlueprintQuantity(axe, 1)]), Datasettings.Default);

        // The nails leave 2 Bronze; the axe takes them and crafts once more for its third, leaving 2 again.
        BlueprintNode axeBronze = totals.Roots[1].Children.Single();
        axeBronze.Should().Match<BlueprintNode>(node =>
            node.Quantity == 3 && node.FromSurplus == 2 && node.Crafts == 1 && node.Surplus == 2);
        axeBronze.Children.Single().Quantity.Should().Be(2); // Copper for the one craft
        totals.Surplus.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(2);
        totals.Materials.ComponentList.Single().Quantity.Should().Be(4);
    }

    [Test]
    public void CalculateTotals_SharedSurplus_CoversABatchEntry()
    {
        BlueprintModel bracket = NewBlueprint("Bracket", 5, yield: 4);
        bracket.Components.Add(NewComponent("Screw"), 1);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 1), new BlueprintQuantity(bracket, 2)]), Datasettings.Default);

        totals.Roots[1].Should().Match<BlueprintNode>(node =>
            node.Quantity == 2 && node.FromSurplus == 2 && node.Crafts == 0 && node.Children.Count == 0);
        totals.TotalValue.Should().Be(10); // the batch still asked for 2 Brackets
        totals.Surplus.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(1);
        totals.SurplusValue.Should().Be(5);
    }

    [Test]
    public void CalculateTotals_SharedSurplus_TotalsDoNotDependOnBatchOrder()
    {
        BlueprintModel bronze = NewBlueprint("Bronze", yield: 3);
        bronze.Components.Add(NewComponent("Copper", 2), 2);

        BlueprintModel nails = NewBlueprint("Bronze Nails", 4);
        nails.ChildBlueprints.Add(bronze, 1);

        BlueprintModel axe = NewBlueprint("Bronze Axe", 10);
        axe.ChildBlueprints.Add(bronze, 5);
        axe.ChildBlueprints.Add(nails, 2);

        // 19 Bronze in all, wherever it is asked for: 7 crafts, 2 left over.
        BlueprintQuantity[] batch = [new(nails, 3), new(axe, 2), new(bronze, 2)];

        BatchTotals forward = BatchProcessor.CalculateTotals(BatchOf(batch), Datasettings.Default);
        BatchTotals backward = BatchProcessor.CalculateTotals(BatchOf([.. batch.Reverse()]), Datasettings.Default);

        forward.Surplus.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(2);
        backward.Crafts.Should().Be(forward.Crafts);
        backward.TotalCost.Should().Be(forward.TotalCost);
        backward.SurplusValue.Should().Be(forward.SurplusValue);
        backward.Surplus.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_YieldNotUsed_NothingComesFromSurplus()
    {
        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 1), new BlueprintQuantity(bracket, 1)]), NoYield);

        totals.Crafts.Should().Be(3);
        totals.Roots.Should().AllSatisfy(root => root.FromSurplus.Should().Be(0));
    }

    [Test]
    public void CalculateTotals_Surplus_IsReportedSeparatelyFromTotalValue()
    {
        BlueprintModel blueprint = NewBlueprint("Frame", 10, yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 1);

        BlueprintQuantity batchEntry = new(blueprint, 3);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([batchEntry]), Datasettings.Default);

        totals.TotalValue.Should().Be(30); // the 3 asked for, not the 4 produced
        totals.Surplus.BlueprintList.Should().ContainSingle().Subject.Quantity.Should().Be(1);
        totals.SurplusValue.Should().Be(10);
    }

    [Test]
    public void CalculateTotals_ValuesNotUsed_SurplusIsWorthNothingButStillCounted()
    {
        BlueprintModel blueprint = NewBlueprint("Frame", 10, yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 3)]), new Datasettings(UseValues: false));

        totals.SurplusValue.Should().Be(0);
        totals.Surplus.BlueprintList.Should().ContainSingle().Subject.Quantity.Should().Be(1);
    }

    [Test]
    public void CalculateTotals_NoProductionTimeAnywhere_IsZero()
    {
        BlueprintModel blueprint = NewBlueprint("Widget", 10);
        blueprint.Components.Add(NewComponent("Screw", 0.5), 2);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 3)]), Datasettings.Default);

        totals.TotalProductionTime.Should().Be(TimeSpan.Zero);
    }

    /// <summary>
    /// The acceptance criteria's farming case: a raw component that still costs time. A Potato has to be
    /// grown before the Soup can be cooked, so its time counts toward the batch.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_CountsComponentsAndBlueprintsTogether()
    {
        ComponentModel potato = NewComponent("Potato");
        potato.ProductionTime = TimeSpan.FromMinutes(5);

        BlueprintModel soup = NewBlueprint("Soup", 10);
        soup.ProductionTime = TimeSpan.FromSeconds(30);
        soup.Components.Add(potato, 2);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(soup, 1)]), Datasettings.Default);

        // Two Potatoes at five minutes each, plus one thirty-second Soup craft.
        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(630));
    }

    /// <summary>
    /// Component time follows the quantity needed, while blueprint time follows the craft count, so a
    /// yield above one reduces the second without touching the first.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_ScalesComponentsByQuantityAndBlueprintsByCrafts()
    {
        ComponentModel screw = NewComponent("Screw");
        screw.ProductionTime = TimeSpan.FromSeconds(1);

        BlueprintModel bracket = NewBlueprint("Bracket", 10, yield: 2);
        bracket.ProductionTime = TimeSpan.FromSeconds(10);
        bracket.Components.Add(screw, 3);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(bracket, 4)]), Datasettings.Default);

        // Four Brackets is two crafts: 2 x 10s of craft time, and 2 x 3 Screws at 1s each.
        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(26));
    }

    [Test]
    public void CalculateTotals_ProductionTime_SumsAcrossTheWholeBatch()
    {
        BlueprintModel left = NewBlueprint("Left", 10);
        left.ProductionTime = TimeSpan.FromSeconds(5);

        BlueprintModel right = NewBlueprint("Right", 10);
        right.ProductionTime = TimeSpan.FromSeconds(7);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(left, 2), new BlueprintQuantity(right, 3)]), Datasettings.Default);

        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(31)); // 2x5s + 3x7s
    }

    /// <summary>
    /// Tenth-of-a-second entries have to survive being multiplied and summed, since that granularity is
    /// the reason production time is stored as ticks rather than as a count of seconds.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_KeepsTenthsExactAcrossManyCrafts()
    {
        BlueprintModel blueprint = NewBlueprint("Widget", 1);
        blueprint.ProductionTime = TimeSpan.FromSeconds(5.5);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, 10)]), Datasettings.Default);

        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(55));
    }

    /// <summary>
    /// The batch quantity field has no upper bound, so a long enough production time and a large enough
    /// quantity reach the point where the tick multiplication wraps negative and the TimeSpan additions
    /// throw. DurationMath saturates both, so the total stays a large duration.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_SaturatesInsteadOfWrappingOnAnAbsurdBatch()
    {
        ComponentModel screw = NewComponent("Screw");
        screw.ProductionTime = TimeSpan.FromHours(24);

        BlueprintModel blueprint = NewBlueprint("Widget", 1);
        blueprint.ProductionTime = TimeSpan.FromHours(24);
        blueprint.Components.Add(screw, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(blueprint, long.MaxValue)]), Datasettings.Default);

        totals.TotalProductionTime.Should().Be(TimeSpan.MaxValue);
    }

    [Test]
    public void CalculateTotals_SingleLevel_NeedsTheBlueprintsOwnComponents()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);
        blueprint.Components.Add(NewComponent("Plate"), 1);

        BatchTotals totals = TotalsOf(blueprint, 1);

        totals.Materials.ComponentList.Select(componentQuantity => (componentQuantity.Name, componentQuantity.Quantity))
            .Should().Equal(("Screw", 2L), ("Plate", 1L));
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_ZeroQuantity_NeedsNothingAndLeavesNoSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Widget", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 2);

        BatchTotals totals = TotalsOf(blueprint, 0);

        // A quantity of zero still lists the component, at zero, the way it did before yield existed.
        totals.Materials.ComponentList.Should().ContainSingle().Subject.Quantity.Should().Be(0);
        totals.Surplus.BlueprintList.Should().BeEmpty();
        totals.Crafts.Should().Be(0);
    }

    [Test]
    public void CalculateTotals_ThreeLevels_MultipliesQuantityAcrossEveryLevel()
    {
        BlueprintModel grandchild = NewBlueprint("Rivet Set");
        grandchild.Components.Add(NewComponent("Rivet"), 1);

        BlueprintModel child = NewBlueprint("Bracket");
        child.ChildBlueprints.Add(grandchild, 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        ComponentQuantity rivets = TotalsOf(parent, 1).Materials.ComponentList.Should().ContainSingle().Subject;

        rivets.Name.Should().Be("Rivet");
        rivets.Quantity.Should().Be(6); // 1 x3 Rivet Sets x2 Brackets
    }

    [Test]
    public void CalculateTotals_Diamond_CombinesSharedComponentFromBothBranches()
    {
        BlueprintModel left = NewBlueprint("Left Arm");
        left.Components.Add(NewComponent("Bolt"), 1);

        BlueprintModel right = NewBlueprint("Right Arm");
        right.Components.Add(NewComponent("Bolt"), 1);

        BlueprintModel parent = NewBlueprint("Chassis");
        parent.ChildBlueprints.Add(left, 1);
        parent.ChildBlueprints.Add(right, 1);

        ComponentQuantity bolts = TotalsOf(parent, 1).Materials.ComponentList.Should().ContainSingle().Subject;

        bolts.Name.Should().Be("Bolt");
        bolts.Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_YieldWithRemainder_RoundsUpAndRecordsTheSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        BatchTotals totals = TotalsOf(blueprint, 3);

        totals.Materials.ComponentList[0].Quantity.Should().Be(6); // 3 Screws x2 crafts to reach 3 Brackets
        BlueprintQuantity spare = totals.Surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Bracket");
        spare.Quantity.Should().Be(1); // 2 crafts produce 4, only 3 were asked for
    }

    [Test]
    public void CalculateTotals_NestedYield_AppliesAtEveryLevel()
    {
        BlueprintModel child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 1);

        BatchTotals totals = TotalsOf(parent, 4);

        totals.Materials.ComponentList.Should().ContainSingle().Subject.Quantity.Should().Be(6); // 4 Brackets -> 2 crafts -> 3 Screws each
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_YieldNotUsed_CraftsOncePerItemAtEveryLevel()
    {
        BlueprintModel child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame", yield: 4);
        parent.ChildBlueprints.Add(child, 1);

        BatchTotals totals = TotalsOf(parent, 3, NoYield);

        totals.Materials.ComponentList.Should().ContainSingle().Subject.Quantity.Should().Be(9); // 3 Frames -> 3 Brackets -> 3 Screws each
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_YieldNotUsed_LeavesTheStoredYieldAlone()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);

        TotalsOf(blueprint, 3, NoYield);

        blueprint.Yield.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_AcceptanceCriteriaExample_OneFrameTakesOneBracketCraft()
    {
        // The worked example from the feature request: a Bracket yielding 2, with a Frame needing two.
        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);
        bracket.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        BatchTotals totals = TotalsOf(frame, 1);

        totals.Materials.ComponentList[0].Quantity.Should().Be(3); // one Bracket craft covers both Brackets
        totals.Surplus.BlueprintList.Should().BeEmpty();
        // One Frame craft plus one Bracket craft.
        totals.Crafts.Should().Be(2);
        totals.Roots.Should().ContainSingle().Subject.Crafts.Should().Be(1);
        totals.Roots[0].Children.Should().ContainSingle().Subject.Crafts.Should().Be(1);
    }

    // A yield above 1 leaves a spare at every level, which the nested copy of the same blueprint must not be able to
    // take in place of crafting.
    [TestCase(1)]
    [TestCase(2)]
    public void CalculateTotals_Cycle_ThrowsInsteadOfOverflowingTheStack(long yield)
    {
        BlueprintModel blueprint = NewBlueprint("Self Referencing", yield: yield);
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => TotalsOf(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// One BlueprintModel can sit in several trees at once, since a loaded dataset shares each blueprint
    /// between every tree it is part of. Adding the totals into the blueprint's own parts would change
    /// every tree that shares it, and the next recalculation would count them again.
    /// </summary>
    [Test]
    public void CalculateTotals_BlueprintSharedBetweenTrees_LeavesItsPartsAsTheyWere()
    {
        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);
        bracket.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        BlueprintQuantity[] batch = [new(frame, 3), new(bracket, 5)];

        BatchTotals first = BatchProcessor.CalculateTotals(BatchOf(batch), Datasettings.Default);
        BatchTotals second = BatchProcessor.CalculateTotals(BatchOf(batch), Datasettings.Default);

        bracket.Components.ComponentList.Should().ContainSingle().Which.Quantity.Should().Be(3);
        frame.ChildBlueprints.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(2);
        second.Materials.ComponentList.Single().Quantity.Should().Be(first.Materials.ComponentList.Single().Quantity);
        second.Surplus.BlueprintList.Single().Quantity.Should().Be(first.Surplus.BlueprintList.Single().Quantity);
    }

    [Test]
    public void CalculateTotals_Roots_AreOnePerBatchEntryInBatchOrder()
    {
        BlueprintModel bronze = NewBlueprint("Bronze");
        BlueprintModel axe = NewBlueprint("Axe");

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(bronze, 2), new BlueprintQuantity(axe, 1)]), Datasettings.Default);

        totals.Roots.Select(root => (root.Name, root.Quantity)).Should().Equal(("Bronze", 2L), ("Axe", 1L));
    }

    [Test]
    public void CalculateTotals_Crafts_CountsEveryCraftAtEveryDepth()
    {
        BlueprintModel bracket = NewBlueprint("Bracket");
        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        // Four Frames that each need two Brackets: four Frame crafts plus eight Bracket crafts.
        TotalsOf(frame, 4).Crafts.Should().Be(12);
    }

    [Test]
    public void CalculateTotals_Crafts_IsTheSumOfEveryNodesCrafts()
    {
        BlueprintModel rivets = NewBlueprint("Rivet Set", yield: 5);
        rivets.Components.Add(NewComponent("Rivet"), 1);

        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);
        bracket.ChildBlueprints.Add(rivets, 3);
        bracket.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 3);
        frame.ChildBlueprints.Add(rivets, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 7), new BlueprintQuantity(bracket, 3)]), Datasettings.Default);

        totals.Crafts.Should().Be(totals.Roots.Sum(SumOfCrafts));
        return;

        static long SumOfCrafts(BlueprintNode node) => node.Crafts + node.Children.Sum(SumOfCrafts);
    }

    /// <summary>
    /// The batch quantity field has no upper bound, so two entries at the largest quantity it takes need more crafts
    /// than a long can count. The step count throws rather than wrapping to a negative number of steps.
    /// </summary>
    [Test]
    public void CalculateTotals_CraftsPastLongMaxValue_ThrowsRatherThanWrapping()
    {
        BlueprintModel left = NewBlueprint("Left");
        BlueprintModel right = NewBlueprint("Right");

        Action act = () => BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(left, long.MaxValue), new BlueprintQuantity(right, 1)]), Datasettings.Default);

        act.Should().Throw<OverflowException>();
    }

    /// <summary>
    /// Valheim: a Bronze Axe is 8 Bronze, 4 Wood and 2 Leather Scraps, and Bronze is 2 Copper and 1 Tin.
    /// </summary>
    private static BlueprintModel NewBronzeAxe(out BlueprintModel bronze)
    {
        bronze = NewBlueprint("Bronze", 6);
        bronze.ProductionTime = TimeSpan.FromMinutes(1);
        bronze.Components.Add(NewComponent("Copper", 2), 2);
        bronze.Components.Add(NewComponent("Tin", 1), 1);

        BlueprintModel axe = NewBlueprint("Bronze Axe", 40);
        axe.ProductionTime = TimeSpan.FromSeconds(10);
        axe.Components.Add(NewComponent("Wood", 0.5), 4);
        axe.Components.Add(NewComponent("Leather Scraps", 1), 2);
        axe.ChildBlueprints.Add(bronze, 8);
        return axe;
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_IsALeafListedWithTheTotalTheBatchNeeds()
    {
        BlueprintModel axe = NewBronzeAxe(out BlueprintModel bronze);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(axe, 3)], bronze), Datasettings.Default);

        totals.Materials.ComponentList.Select(componentQuantity => (componentQuantity.Name, componentQuantity.Quantity))
            .Should().Equal(("Wood", 12L), ("Leather Scraps", 6L));
        totals.AlreadyCrafted.BlueprintList.Select(blueprintQuantity => (blueprintQuantity.Name, blueprintQuantity.Quantity))
            .Should().Equal(("Bronze", 24L));
        totals.Crafts.Should().Be(3); // the axes only, rather than 27 with the Bronze
        BlueprintNode bronzeNode = totals.Roots.Single().Children.Single(node => node.Source == bronze);
        bronzeNode.Should().Match<BlueprintNode>(node =>
            node.AlreadyCrafted && node.IsComponent && node.Quantity == 24 && node.Crafts == 0 && node.Yield == 0
            && node.Surplus == 0 && node.ProductionTime == TimeSpan.Zero && node.Children.Count == 0);
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_SumsEveryPositionIntoOneEntry()
    {
        // Karve prep: one Bronze craft makes 20 Bronze Nails, so 10 nails take 1 Bronze, and the axe takes 8.
        BlueprintModel axe = NewBronzeAxe(out BlueprintModel bronze);
        BlueprintModel nails = NewBlueprint("Bronze Nails", 1, yield: 20);
        nails.ChildBlueprints.Add(bronze, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(nails, 10), new BlueprintQuantity(axe, 1)], bronze), Datasettings.Default);

        totals.AlreadyCrafted.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(9);
        totals.Crafts.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_CostsItsValue()
    {
        BlueprintModel axe = NewBronzeAxe(out BlueprintModel bronze);
        Batch batch = BatchOf([new BlueprintQuantity(axe, 3)], bronze);

        // 24 Bronze at 6, 12 Wood at 0.5 and 6 Leather Scraps at 1.
        BatchProcessor.CalculateTotals(batch, Datasettings.Default).TotalCost.Should().Be(156);
        // Without values the Bronze is worth nothing, so only the Wood and the Leather Scraps cost anything.
        BatchProcessor.CalculateTotals(batch, new Datasettings(UseValues: false)).TotalCost.Should().Be(12);
        BatchProcessor.CalculateTotals(batch, new Datasettings(UseCosts: false)).TotalCost.Should().Be(0);
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_LeavesTheValueAlone()
    {
        BlueprintModel axe = NewBronzeAxe(out BlueprintModel bronze);

        BatchTotals marked = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(axe, 3)], bronze), Datasettings.Default);
        BatchTotals unmarked = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(axe, 3)]), Datasettings.Default);

        marked.TotalValue.Should().Be(120);
        unmarked.TotalValue.Should().Be(120);
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_TakesNoTimeForItsBranch()
    {
        BlueprintModel axe = NewBronzeAxe(out BlueprintModel bronze);
        bronze.Components.ComponentList.Single(componentQuantity => componentQuantity.Name == "Copper")
            .Component.ProductionTime = TimeSpan.FromSeconds(30);

        BatchTotals totals = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(axe, 3)], bronze), Datasettings.Default);

        // Three ten-second Axe crafts. No Bronze crafts, and no Copper.
        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Test]
    public void CalculateTotals_AlreadyCrafted_SiblingsStillShareTheirSurplus()
    {
        // Minecraft: 1 log makes 4 planks, 2 planks make 4 sticks. A wooden pickaxe is 3 planks and 2 sticks,
        // a wooden sword 2 planks and 1 stick. With the sticks already made, only the planks are crafted.
        BlueprintModel planks = NewBlueprint("Planks", yield: 4);
        planks.Components.Add(NewComponent("Log"), 1);

        BlueprintModel sticks = NewBlueprint("Stick", yield: 4);
        sticks.ChildBlueprints.Add(planks, 2);

        BlueprintModel pickaxe = NewBlueprint("Wooden Pickaxe");
        pickaxe.ChildBlueprints.Add(planks, 3);
        pickaxe.ChildBlueprints.Add(sticks, 2);

        BlueprintModel sword = NewBlueprint("Wooden Sword");
        sword.ChildBlueprints.Add(planks, 2);
        sword.ChildBlueprints.Add(sticks, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(pickaxe, 1), new BlueprintQuantity(sword, 1)], sticks), Datasettings.Default);

        // 5 planks in two crafts: the sword takes the pickaxe's spare plank and crafts once for its second.
        totals.Crafts.Should().Be(4);
        totals.Materials.ComponentList.Single().Quantity.Should().Be(2);
        totals.Roots[1].Children[0].Should().Match<BlueprintNode>(node => node.FromSurplus == 1 && node.Crafts == 1);
        totals.Surplus.BlueprintList.Select(spare => (spare.Name, spare.Quantity)).Should().Equal(("Planks", 3L));
        totals.AlreadyCrafted.BlueprintList.Should().ContainSingle().Which.Quantity.Should().Be(3);
    }

    [Test]
    public void CalculateTotals_AlreadyCraftedInsideAnotherOne_HasNoEffect()
    {
        BlueprintModel rivets = NewBlueprint("Rivet Set");
        rivets.Components.Add(NewComponent("Rivet"), 1);

        BlueprintModel bracket = NewBlueprint("Bracket");
        bracket.ChildBlueprints.Add(rivets, 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        BatchTotals both = BatchProcessor.CalculateTotals(
            BatchOf([new BlueprintQuantity(frame, 1)], bracket, rivets), Datasettings.Default);
        BatchTotals outer = BatchProcessor.CalculateTotals(BatchOf([new BlueprintQuantity(frame, 1)], bracket), Datasettings.Default);

        both.AlreadyCrafted.BlueprintList.Select(blueprintQuantity => (blueprintQuantity.Name, blueprintQuantity.Quantity))
            .Should().Equal(("Bracket", 2L));
        both.Crafts.Should().Be(outer.Crafts);
        both.Materials.ComponentList.Should().BeEmpty();
    }

    [Test]
    public void CostOf_Component_IsItsCostUnlessCostsAreOff()
    {
        ComponentQuantity copper = new(NewComponent("Copper", 2), 10);

        BatchProcessor.CostOf(copper, Datasettings.Default).Should().Be(20);
        BatchProcessor.CostOf(copper, new Datasettings(UseValues: false)).Should().Be(20);
        BatchProcessor.CostOf(copper, new Datasettings(UseCosts: false)).Should().Be(0);
    }

    [Test]
    public void CostOf_Blueprint_IsItsValueUnlessCostsOrValuesAreOff()
    {
        BlueprintQuantity bronze = new(NewBlueprint("Bronze", 6), 24);

        BatchProcessor.CostOf(bronze, Datasettings.Default).Should().Be(144);
        BatchProcessor.CostOf(bronze, new Datasettings(UseValues: false)).Should().Be(0);
        BatchProcessor.CostOf(bronze, new Datasettings(UseCosts: false)).Should().Be(0);
    }

    [Test]
    public void CalculateTotals_Tree_NamesEachNodeAfterItsSourceRecord()
    {
        ComponentModel screw = NewComponent("Screw");
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(screw, 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        BlueprintNode tree = TreeOf(parent, 1);

        tree.Name.Should().Be("Frame");
        tree.Source.Should().BeSameAs(parent);
        BlueprintNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Name.Should().Be("Bracket");
        childNode.Source.Should().BeSameAs(child);
        childNode.IsComponent.Should().BeFalse();
        BlueprintNode componentNode = childNode.Children.Should().ContainSingle().Subject;
        componentNode.Name.Should().Be("Screw");
        componentNode.Source.Should().BeSameAs(screw);
        componentNode.IsComponent.Should().BeTrue();
    }

    [Test]
    public void CalculateTotals_Tree_ListsComponentsBeforeChildBlueprints()
    {
        BlueprintModel bracket = NewBlueprint("Bracket");
        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 1);
        frame.Components.Add(NewComponent("Screw"), 1);

        TreeOf(frame, 1).Children.Select(node => node.Name).Should().Equal("Screw", "Bracket");
    }

    [Test]
    public void CalculateTotals_Tree_CarriesTheEffectiveQuantityThroughEveryLevel()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        // Four Frames, each needing two Brackets, each needing three Screws.
        BlueprintNode tree = TreeOf(parent, 4);

        tree.Quantity.Should().Be(4);
        BlueprintNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Quantity.Should().Be(8);
        childNode.Children.Should().ContainSingle().Subject.Quantity.Should().Be(24);
    }

    [Test]
    public void CalculateTotals_Tree_DefaultYieldCountsOneCraftPerUnit()
    {
        BlueprintModel blueprint = NewBlueprint("Frame");
        blueprint.Components.Add(NewComponent("Screw"), 1);

        BlueprintNode tree = TreeOf(blueprint, 4);

        tree.Crafts.Should().Be(4);
        BlueprintProcessor.CountsByCraft(tree).Should().BeFalse();
        tree.Children.Should().ContainSingle().Subject.Crafts.Should().Be(0); // components are gathered, not crafted
    }

    [Test]
    public void CalculateTotals_Tree_YieldReportsTheCraftsThatCoverTheQuantity()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);

        BlueprintNode three = TreeOf(blueprint, 3);
        three.Quantity.Should().Be(3);
        three.Crafts.Should().Be(2);

        BlueprintNode two = TreeOf(blueprint, 2);
        two.Quantity.Should().Be(2);
        two.Crafts.Should().Be(1);
        BlueprintProcessor.CountsByCraft(two).Should().BeTrue();
    }

    /// <summary>
    /// The step dialog shows yield beside surplus, because the surplus is only explicable as the
    /// remainder of rounding the quantity up to a whole number of crafts of that size.
    /// </summary>
    [Test]
    public void CalculateTotals_Tree_CarriesTheYieldThatExplainsTheSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 3);

        BlueprintNode node = TreeOf(blueprint, 4);

        node.Yield.Should().Be(3);
        node.Crafts.Should().Be(2);
        // Two crafts of three make six, so four leaves two spare.
        node.Surplus.Should().Be(2);
        (node.Crafts * node.Yield).Should().Be(node.Quantity + node.Surplus);
        TreeOf(blueprint, 6).Surplus.Should().Be(0);
    }

    [Test]
    public void CalculateTotals_Tree_YieldNotUsedReportsOneItemPerCraftAndNoSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        BlueprintNode node = TreeOf(blueprint, 3, NoYield);

        node.Crafts.Should().Be(3);
        node.Yield.Should().Be(1);
        node.Surplus.Should().Be(0);
        node.Children.Should().ContainSingle().Subject.Quantity.Should().Be(9);
        BlueprintProcessor.CountsByCraft(node).Should().BeFalse();
    }

    [Test]
    public void CalculateTotals_Tree_ComponentLeafHasNoCraftsYieldOrSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);

        BlueprintNode leaf = TreeOf(blueprint, 1).Children.Should().ContainSingle().Subject;

        leaf.IsComponent.Should().BeTrue();
        leaf.Crafts.Should().Be(0);
        leaf.Yield.Should().Be(0);
        leaf.Surplus.Should().Be(0);
    }

    [Test]
    public void CalculateTotals_Tree_YieldScalesChildrenByCraftsRatherThanUnits()
    {
        BlueprintModel child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 1);

        BlueprintNode childNode = TreeOf(parent, 4).Children.Should().ContainSingle().Subject;

        childNode.Quantity.Should().Be(4); // still four Brackets needed
        childNode.Crafts.Should().Be(2);   // but only two crafts to make them
        childNode.Children.Should().ContainSingle().Subject.Quantity.Should().Be(6); // 3 Screws x2 crafts
    }

    [Test]
    public void CalculateTotals_Tree_HundredOfAYieldTwoBlueprintIsFiftyCrafts()
    {
        BlueprintNode tree = TreeOf(NewBlueprint("Frame", yield: 2), 100);

        tree.Quantity.Should().Be(100);
        tree.Crafts.Should().Be(50);
    }

    [Test]
    public void CalculateTotals_Tree_ProductionTimeIsTheRowsOwnTimeExcludingChildren()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.ProductionTime = TimeSpan.FromSeconds(2);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ProductionTime = TimeSpan.FromSeconds(5);
        parent.ChildBlueprints.Add(child, 3);

        BlueprintNode tree = TreeOf(parent, 1);

        tree.ProductionTime.Should().Be(TimeSpan.FromSeconds(5));
        tree.Children.Should().ContainSingle().Subject.ProductionTime.Should().Be(TimeSpan.FromSeconds(6));
    }

    [Test]
    public void CalculateTotals_Tree_ComponentLeafScalesProductionTimeByQuantity()
    {
        ComponentModel potato = NewComponent("Potato");
        potato.ProductionTime = TimeSpan.FromMinutes(5);

        BlueprintModel blueprint = NewBlueprint("Soup");
        blueprint.Components.Add(potato, 2);

        BlueprintNode leaf = TreeOf(blueprint, 3).Children.Should().ContainSingle().Subject;

        leaf.Quantity.Should().Be(6);
        leaf.ProductionTime.Should().Be(TimeSpan.FromMinutes(30));
    }

    /// <summary>
    /// The multiplier is the craft count, not the quantity: a yield of two halves the crafts, so it
    /// halves the time as well.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_FollowsCraftsRatherThanQuantity()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.ProductionTime = TimeSpan.FromSeconds(10);

        TotalsOf(blueprint, 4).TotalProductionTime.Should().Be(TimeSpan.FromSeconds(20));
        // Three still takes two crafts, so it costs the same twenty seconds and leaves one spare.
        TotalsOf(blueprint, 3).TotalProductionTime.Should().Be(TimeSpan.FromSeconds(20));
    }

    [Test]
    public void CalculateTotals_ProductionTime_SumsEveryDepthOfTheGraph()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.ProductionTime = TimeSpan.FromSeconds(2);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ProductionTime = TimeSpan.FromSeconds(5);
        parent.ChildBlueprints.Add(child, 3);

        // One Frame craft (5s) plus three Bracket crafts (6s).
        TotalsOf(parent, 1).TotalProductionTime.Should().Be(TimeSpan.FromSeconds(11));
    }
}
