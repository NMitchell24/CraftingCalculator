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
}
