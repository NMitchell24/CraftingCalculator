using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintProcessorTests
{
    private static Component NewComponent(string name) => new Component { Id = 1, Name = name };

    private static Blueprint NewBlueprint(string name) => new Blueprint { Id = 1, Name = name };

    private static Blueprint NewBlueprint(string name, long yield) => new Blueprint { Id = 1, Name = name, Yield = yield };

    [TestCase(0, 2, ExpectedResult = 0)]
    [TestCase(-5, 2, ExpectedResult = 0)]
    [TestCase(1, 1, ExpectedResult = 1)]
    [TestCase(4, 1, ExpectedResult = 4)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(2, 2, ExpectedResult = 1)]
    [TestCase(3, 2, ExpectedResult = 2)]
    [TestCase(4, 2, ExpectedResult = 2)]
    [TestCase(100, 2, ExpectedResult = 50)]
    [TestCase(10, 3, ExpectedResult = 4)]
    public long CraftsFor_RoundsUpToWholeCrafts(long quantity, long yield) =>
        BlueprintProcessor.CraftsFor(quantity, yield);

    [Test]
    public void Flatten_SingleLevel_ReturnsOwnComponents()
    {
        Blueprint blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);
        blueprint.Components.Add(NewComponent("Plate"), 1);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(blueprint, 1);

        result.ComponentList.Should().HaveCount(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Screw").Quantity.Should().Be(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Plate").Quantity.Should().Be(1);
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_ScalesOwnComponentsByQuantity()
    {
        Blueprint blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);

        (ComponentMap result, _) = BlueprintProcessor.Flatten(blueprint, 3);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
    }

    [Test]
    public void Flatten_ZeroQuantity_NeedsNothingAndLeavesNoSurplus()
    {
        Blueprint blueprint = NewBlueprint("Widget", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 2);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(blueprint, 0);

        // A quantity of zero still lists the component, at zero, the way it did before yield existed.
        result.ComponentList.Should().ContainSingle().Subject.Quantity.Should().Be(0);
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_TwoLevels_ScalesChildComponentsByChildQuantity()
    {
        Blueprint child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        (ComponentMap result, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Screw");
        result.ComponentList[0].Quantity.Should().Be(6); // 3 per Bracket x2 Brackets
    }

    [Test]
    public void Flatten_ThreeLevels_MultipliesQuantityAcrossEveryLevel()
    {
        Blueprint grandchild = NewBlueprint("Rivet Set");
        grandchild.Components.Add(NewComponent("Rivet"), 1);

        Blueprint child = NewBlueprint("Bracket");
        child.ChildBlueprints.Add(grandchild, 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        (ComponentMap result, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Rivet");
        result.ComponentList[0].Quantity.Should().Be(6); // 1 x3 Rivet Sets x2 Brackets
    }

    [Test]
    public void Flatten_Diamond_CombinesSharedComponentFromBothBranches()
    {
        Blueprint left = NewBlueprint("Left Arm");
        left.Components.Add(NewComponent("Bolt"), 1);

        Blueprint right = NewBlueprint("Right Arm");
        right.Components.Add(NewComponent("Bolt"), 1);

        Blueprint parent = NewBlueprint("Chassis");
        parent.ChildBlueprints.Add(left, 1);
        parent.ChildBlueprints.Add(right, 1);

        (ComponentMap result, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Bolt");
        result.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void Flatten_YieldDividesEvenly_ChargesOnlyForTheCraftsNeeded()
    {
        Blueprint blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(blueprint, 4);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 3 Screws x2 crafts, not x4 Brackets
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_YieldWithRemainder_RoundsUpAndRecordsTheSurplus()
    {
        Blueprint blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(blueprint, 3);

        result.ComponentList[0].Quantity.Should().Be(6); // 3 Screws x2 crafts to reach 3 Brackets
        BlueprintQuantity spare = surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Bracket");
        spare.Quantity.Should().Be(1); // 2 crafts produce 4, only 3 were asked for
    }

    [Test]
    public void Flatten_NestedYield_AppliesAtEveryLevel()
    {
        Blueprint child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 1);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(parent, 4);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 4 Brackets -> 2 crafts -> 3 Screws each
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_AcceptanceCriteriaExample_OneFrameTakesOneBracketCraft()
    {
        // The worked example from the feature request: a Bracket yielding 2, with a Frame needing two.
        Blueprint bracket = NewBlueprint("Bracket", yield: 2);
        bracket.Components.Add(NewComponent("Screw"), 3);

        Blueprint frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        (ComponentMap result, BlueprintMap surplus) = BlueprintProcessor.Flatten(frame, 1);

        result.ComponentList[0].Quantity.Should().Be(3); // one Bracket craft covers both Brackets
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_TopLevelYieldWithRemainder_RecordsSurplusOfTheBlueprintItself()
    {
        Blueprint blueprint = NewBlueprint("Frame", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 1);

        (_, BlueprintMap surplus) = BlueprintProcessor.Flatten(blueprint, 3);

        BlueprintQuantity spare = surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Frame");
        spare.Quantity.Should().Be(1);
    }

    [Test]
    public void Flatten_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Blueprint blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.Flatten(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void BuildNode_ScalesNodeNamesByQuantity()
    {
        Blueprint child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        BlueprintNode tree = BlueprintProcessor.BuildNode(parent, 1);

        tree.Name.Should().Be("Frame x1");
        BlueprintNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Name.Should().Be("Bracket x2");
        childNode.IsComponent.Should().BeFalse();
        BlueprintNode componentNode = childNode.Children.Should().ContainSingle().Subject;
        componentNode.Name.Should().Be("Screw x6");
        componentNode.IsComponent.Should().BeTrue();
    }

    [Test]
    public void BuildNode_CarriesTheEffectiveQuantityThroughEveryLevel()
    {
        Blueprint child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        // Four Frames, each needing two Brackets, each needing three Screws.
        BlueprintNode tree = BlueprintProcessor.BuildNode(parent, 4);

        tree.Quantity.Should().Be(4);
        BlueprintNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Quantity.Should().Be(8);
        BlueprintNode componentNode = childNode.Children.Should().ContainSingle().Subject;
        componentNode.Quantity.Should().Be(24);
    }

    [Test]
    public void BuildNode_DefaultYield_CountsOneCraftPerUnitAndOmitsTheSuffix()
    {
        Blueprint blueprint = NewBlueprint("Frame");
        blueprint.Components.Add(NewComponent("Screw"), 1);

        BlueprintNode tree = BlueprintProcessor.BuildNode(blueprint, 4);

        tree.Name.Should().Be("Frame x4");
        tree.Crafts.Should().Be(4);
        tree.Children.Should().ContainSingle().Subject.Crafts.Should().Be(0); // components are gathered, not crafted
    }

    [Test]
    public void BuildNode_Yield_LabelsTheCraftCountBesideTheQuantity()
    {
        Blueprint blueprint = NewBlueprint("Bracket", yield: 2);

        BlueprintProcessor.BuildNode(blueprint, 3).Name.Should().Be("Bracket x3 (2 crafts)");
        BlueprintProcessor.BuildNode(blueprint, 2).Name.Should().Be("Bracket x2 (1 craft)");
    }

    [Test]
    public void BuildNode_Yield_ScalesChildrenByCraftsRatherThanUnits()
    {
        Blueprint child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 1);

        BlueprintNode tree = BlueprintProcessor.BuildNode(parent, 4);

        BlueprintNode childNode = tree.Children.Should().ContainSingle().Subject;
        childNode.Quantity.Should().Be(4); // still four Brackets needed
        childNode.Crafts.Should().Be(2);   // but only two crafts to make them
        childNode.Children.Should().ContainSingle().Subject.Quantity.Should().Be(6); // 3 Screws x2 crafts
    }

    [Test]
    public void BuildNode_AcceptanceCriteriaExample_IsOneCraftAtEachLevel()
    {
        Blueprint bracket = NewBlueprint("Bracket", yield: 2);

        Blueprint frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        BlueprintNode tree = BlueprintProcessor.BuildNode(frame, 1);

        // What CraftState.CraftingStepCount sums: one Frame craft plus one Bracket craft.
        tree.Crafts.Should().Be(1);
        tree.Children.Should().ContainSingle().Subject.Crafts.Should().Be(1);
    }

    [Test]
    public void BuildNode_HundredOfAYieldTwoBlueprint_IsFiftyCrafts()
    {
        Blueprint frame = NewBlueprint("Frame", yield: 2);

        BlueprintNode tree = BlueprintProcessor.BuildNode(frame, 100);

        tree.Quantity.Should().Be(100);
        tree.Crafts.Should().Be(50);
        tree.Name.Should().Be("Frame x100 (50 crafts)");
    }

    [Test]
    public void BuildNode_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Blueprint blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.BuildNode(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }
}
