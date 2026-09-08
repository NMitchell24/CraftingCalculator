using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BatchProcessorTests
{
    private static Component NewComponent(string name, double cost) => new Component { Id = 1, Name = name, Cost = cost };

    private static Blueprint NewBlueprint(string name, double value) => new Blueprint { Id = 1, Name = name, Value = value };

    private static Blueprint NewBlueprint(string name, double value, long yield) =>
        new Blueprint { Id = 1, Name = name, Value = value, Yield = yield };

    [Test]
    public void CalculateTotals_SingleBlueprint_ReturnsCostAndValue()
    {
        Blueprint blueprint = NewBlueprint("Widget", 10);
        blueprint.Components.Add(NewComponent("Screw", 0.5), 2);

        BlueprintQuantity batchEntry = new(blueprint, 3, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([batchEntry]);

        totals.TotalCost.Should().Be(3.0); // 0.5 x2 Screws x3 Widgets
        totals.TotalValue.Should().Be(30); // 10 x3 Widgets
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_NestedBlueprint_ScalesChildComponentsByBatchAndParentQuantity()
    {
        Blueprint child = NewBlueprint("Bracket", 0);
        child.Components.Add(NewComponent("Screw", 1), 3);

        Blueprint parent = NewBlueprint("Frame", 0);
        parent.ChildBlueprints.Add(child, 2);

        BlueprintQuantity batchEntry = new(parent, 2, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([batchEntry]);

        totals.TotalCost.Should().Be(12); // 1 x3 Screws x2 Brackets x2 Frames
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(12);
    }

    [Test]
    public void CalculateTotals_TwoBlueprintsSharingComponent_MergesMaterialsAndSumsValue()
    {
        Blueprint left = NewBlueprint("Left Arm", 5);
        left.Components.Add(NewComponent("Bolt", 1), 1);

        Blueprint right = NewBlueprint("Right Arm", 7);
        right.Components.Add(NewComponent("Bolt", 1), 1);

        BlueprintQuantity leftEntry = new(left, 1, id: 0);
        BlueprintQuantity rightEntry = new(right, 1, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([leftEntry, rightEntry]);

        totals.TotalCost.Should().Be(2);
        totals.TotalValue.Should().Be(12);
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_ZeroCostComponent_ContributesNothingToCost()
    {
        Blueprint blueprint = NewBlueprint("Widget", 0);
        blueprint.Components.Add(NewComponent("Free Sample", 0), 5);

        BlueprintQuantity batchEntry = new(blueprint, 1, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([batchEntry]);

        totals.TotalCost.Should().Be(0);
        totals.Materials.ComponentList.Should().ContainSingle();
        totals.Materials.ComponentList[0].Quantity.Should().Be(5);
    }

    [Test]
    public void CalculateTotals_Yield_ChargesOnlyForTheCraftsNeeded()
    {
        Blueprint blueprint = NewBlueprint("Bracket", 0, yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 3);

        BlueprintQuantity batchEntry = new(blueprint, 4, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([batchEntry]);

        totals.TotalCost.Should().Be(6); // 3 Screws x2 crafts, not x4 Brackets
        totals.Materials.ComponentList[0].Quantity.Should().Be(6);
        totals.Surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void CalculateTotals_TwoBlueprintsNestingTheSameYieldingChild_RoundEachPositionIndependently()
    {
        // Each parent needs one Bracket, so each runs its own Bracket craft rather than sharing one
        // between them - and each craft leaves a spare.
        Blueprint bracket = NewBlueprint("Bracket", 5, yield: 2);
        bracket.Components.Add(NewComponent("Screw", 1), 3);

        Blueprint frame = NewBlueprint("Frame", 0);
        frame.ChildBlueprints.Add(bracket, 1);

        Blueprint handle = NewBlueprint("Handle", 0);
        handle.ChildBlueprints.Add(bracket, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals([new(frame, 1, id: 0), new(handle, 1, id: 0)]);

        totals.TotalCost.Should().Be(6); // two Bracket crafts, 3 Screws each
        BlueprintQuantity spare = totals.Surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Bracket");
        spare.Quantity.Should().Be(2);
        spare.TotalValue.Should().Be(10);
    }

    [Test]
    public void CalculateTotals_Surplus_IsReportedSeparatelyFromTotalValue()
    {
        Blueprint blueprint = NewBlueprint("Frame", 10, yield: 2);
        blueprint.Components.Add(NewComponent("Screw", 1), 1);

        BlueprintQuantity batchEntry = new(blueprint, 3, id: 0);

        BatchTotals totals = BatchProcessor.CalculateTotals([batchEntry]);

        totals.TotalValue.Should().Be(30); // the 3 asked for, not the 4 produced
        totals.Surplus.BlueprintList.Should().ContainSingle().Subject.Quantity.Should().Be(1);
    }

    [Test]
    public void CalculateTotals_NoProductionTimeAnywhere_IsZero()
    {
        Blueprint blueprint = NewBlueprint("Widget", 10);
        blueprint.Components.Add(NewComponent("Screw", 0.5), 2);

        BatchTotals totals = BatchProcessor.CalculateTotals([new BlueprintQuantity(blueprint, 3, id: 0)]);

        totals.TotalProductionTime.Should().Be(TimeSpan.Zero);
    }

    /// <summary>
    /// The acceptance criteria's farming case: a raw component that still costs time. A Potato has to be
    /// grown before the Soup can be cooked, so its time counts toward the batch.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_CountsComponentsAndBlueprintsTogether()
    {
        Component potato = NewComponent("Potato", 0);
        potato.ProductionTime = TimeSpan.FromMinutes(5);

        Blueprint soup = NewBlueprint("Soup", 10);
        soup.ProductionTime = TimeSpan.FromSeconds(30);
        soup.Components.Add(potato, 2);

        BatchTotals totals = BatchProcessor.CalculateTotals([new BlueprintQuantity(soup, 1, id: 0)]);

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
        Component screw = NewComponent("Screw", 0);
        screw.ProductionTime = TimeSpan.FromSeconds(1);

        Blueprint bracket = NewBlueprint("Bracket", 10, yield: 2);
        bracket.ProductionTime = TimeSpan.FromSeconds(10);
        bracket.Components.Add(screw, 3);

        BatchTotals totals = BatchProcessor.CalculateTotals([new BlueprintQuantity(bracket, 4, id: 0)]);

        // Four Brackets is two crafts: 2 x 10s of craft time, and 2 x 3 Screws at 1s each.
        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(26));
    }

    [Test]
    public void CalculateTotals_ProductionTime_SumsAcrossTheWholeBatch()
    {
        Blueprint left = NewBlueprint("Left", 10);
        left.ProductionTime = TimeSpan.FromSeconds(5);

        Blueprint right = NewBlueprint("Right", 10);
        right.ProductionTime = TimeSpan.FromSeconds(7);

        BatchTotals totals = BatchProcessor.CalculateTotals(
            [new BlueprintQuantity(left, 2, id: 0), new BlueprintQuantity(right, 3, id: 0)]);

        totals.TotalProductionTime.Should().Be(TimeSpan.FromSeconds(31)); // 2x5s + 3x7s
    }

    /// <summary>
    /// Tenth-of-a-second entries have to survive being multiplied and summed, since that granularity is
    /// the reason production time is stored as ticks rather than as a count of seconds.
    /// </summary>
    [Test]
    public void CalculateTotals_ProductionTime_KeepsTenthsExactAcrossManyCrafts()
    {
        Blueprint blueprint = NewBlueprint("Widget", 1);
        blueprint.ProductionTime = TimeSpan.FromSeconds(5.5);

        BatchTotals totals = BatchProcessor.CalculateTotals([new BlueprintQuantity(blueprint, 10, id: 0)]);

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
        Component screw = NewComponent("Screw", 0);
        screw.ProductionTime = TimeSpan.FromHours(24);

        Blueprint blueprint = NewBlueprint("Widget", 1);
        blueprint.ProductionTime = TimeSpan.FromHours(24);
        blueprint.Components.Add(screw, 1);

        BatchTotals totals = BatchProcessor.CalculateTotals([new BlueprintQuantity(blueprint, long.MaxValue, id: 0)]);

        totals.TotalProductionTime.Should().Be(TimeSpan.MaxValue);
    }
}
