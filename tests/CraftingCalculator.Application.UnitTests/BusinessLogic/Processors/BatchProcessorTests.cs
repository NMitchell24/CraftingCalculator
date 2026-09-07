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

    [Test]
    public void CalculateTotals_SingleBlueprint_ReturnsCostAndValue()
    {
        Blueprint blueprint = NewBlueprint("Widget", 10);
        blueprint.Components.Add(NewComponent("Screw", 0.5), 2);

        BlueprintQuantity batchEntry = new(blueprint, 3, id: 0);

        (double totalCost, double totalValue, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(3.0); // 0.5 x2 Screws x3 Widgets
        totalValue.Should().Be(30); // 10 x3 Widgets
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
    }

    [Test]
    public void CalculateTotals_NestedBlueprint_ScalesChildComponentsByBatchAndParentQuantity()
    {
        Blueprint child = NewBlueprint("Bracket", 0);
        child.Components.Add(NewComponent("Screw", 1), 3);

        Blueprint parent = NewBlueprint("Frame", 0);
        parent.ChildBlueprints.Add(child, 2);

        BlueprintQuantity batchEntry = new(parent, 2, id: 0);

        (double totalCost, _, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(12); // 1 x3 Screws x2 Brackets x2 Frames
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(12);
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

        (double totalCost, double totalValue, ComponentMap materials) =
            BatchProcessor.CalculateTotals([leftEntry, rightEntry]);

        totalCost.Should().Be(2);
        totalValue.Should().Be(12);
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void CalculateTotals_ZeroCostComponent_ContributesNothingToCost()
    {
        Blueprint blueprint = NewBlueprint("Widget", 0);
        blueprint.Components.Add(NewComponent("Free Sample", 0), 5);

        BlueprintQuantity batchEntry = new(blueprint, 1, id: 0);

        (double totalCost, _, ComponentMap materials) = BatchProcessor.CalculateTotals([batchEntry]);

        totalCost.Should().Be(0);
        materials.ComponentList.Should().ContainSingle();
        materials.ComponentList[0].Quantity.Should().Be(5);
    }
}
