using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintProcessorTests
{
    private static ComponentModel NewComponent(string name) => new ComponentModel { Id = 1, Name = name };

    private static BlueprintModel NewBlueprint(string name) => new BlueprintModel { Id = 1, Name = name };

    private static BlueprintModel NewBlueprint(string name, long yield) => new BlueprintModel { Id = 1, Name = name, Yield = yield };

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

    [TestCase(0)]
    [TestCase(-1)]
    public void CraftsFor_YieldBelowOne_Throws(long yield)
    {
        Action act = () => BlueprintProcessor.CraftsFor(4, yield);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void Flatten_SingleLevel_ReturnsOwnComponents()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);
        blueprint.Components.Add(NewComponent("Plate"), 1);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(blueprint, 1);

        result.ComponentList.Should().HaveCount(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Screw").Quantity.Should().Be(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Plate").Quantity.Should().Be(1);
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_ScalesOwnComponentsByQuantity()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);

        (ComponentMap result, _, _) = BlueprintProcessor.Flatten(blueprint, 3);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 2 Screws x3 Widgets
    }

    [Test]
    public void Flatten_ZeroQuantity_NeedsNothingAndLeavesNoSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Widget", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 2);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(blueprint, 0);

        // A quantity of zero still lists the component, at zero, the way it did before yield existed.
        result.ComponentList.Should().ContainSingle().Subject.Quantity.Should().Be(0);
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_TwoLevels_ScalesChildComponentsByChildQuantity()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        (ComponentMap result, _, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Screw");
        result.ComponentList[0].Quantity.Should().Be(6); // 3 per Bracket x2 Brackets
    }

    [Test]
    public void Flatten_ThreeLevels_MultipliesQuantityAcrossEveryLevel()
    {
        BlueprintModel grandchild = NewBlueprint("Rivet Set");
        grandchild.Components.Add(NewComponent("Rivet"), 1);

        BlueprintModel child = NewBlueprint("Bracket");
        child.ChildBlueprints.Add(grandchild, 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        (ComponentMap result, _, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Rivet");
        result.ComponentList[0].Quantity.Should().Be(6); // 1 x3 Rivet Sets x2 Brackets
    }

    [Test]
    public void Flatten_Diamond_CombinesSharedComponentFromBothBranches()
    {
        BlueprintModel left = NewBlueprint("Left Arm");
        left.Components.Add(NewComponent("Bolt"), 1);

        BlueprintModel right = NewBlueprint("Right Arm");
        right.Components.Add(NewComponent("Bolt"), 1);

        BlueprintModel parent = NewBlueprint("Chassis");
        parent.ChildBlueprints.Add(left, 1);
        parent.ChildBlueprints.Add(right, 1);

        (ComponentMap result, _, _) = BlueprintProcessor.Flatten(parent, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Bolt");
        result.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void Flatten_YieldDividesEvenly_ChargesOnlyForTheCraftsNeeded()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(blueprint, 4);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 3 Screws x2 crafts, not x4 Brackets
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_YieldWithRemainder_RoundsUpAndRecordsTheSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 3);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(blueprint, 3);

        result.ComponentList[0].Quantity.Should().Be(6); // 3 Screws x2 crafts to reach 3 Brackets
        BlueprintQuantity spare = surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Bracket");
        spare.Quantity.Should().Be(1); // 2 crafts produce 4, only 3 were asked for
    }

    [Test]
    public void Flatten_NestedYield_AppliesAtEveryLevel()
    {
        BlueprintModel child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 1);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(parent, 4);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(6); // 4 Brackets -> 2 crafts -> 3 Screws each
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_AcceptanceCriteriaExample_OneFrameTakesOneBracketCraft()
    {
        // The worked example from the feature request: a Bracket yielding 2, with a Frame needing two.
        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);
        bracket.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        (ComponentMap result, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(frame, 1);

        result.ComponentList[0].Quantity.Should().Be(3); // one Bracket craft covers both Brackets
        surplus.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void Flatten_TopLevelYieldWithRemainder_RecordsSurplusOfTheBlueprintItself()
    {
        BlueprintModel blueprint = NewBlueprint("Frame", yield: 2);
        blueprint.Components.Add(NewComponent("Screw"), 1);

        (_, BlueprintMap surplus, _) = BlueprintProcessor.Flatten(blueprint, 3);

        BlueprintQuantity spare = surplus.BlueprintList.Should().ContainSingle().Subject;
        spare.Name.Should().Be("Frame");
        spare.Quantity.Should().Be(1);
    }

    [Test]
    public void Flatten_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        BlueprintModel blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.Flatten(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void BuildNode_ScalesNodeNamesByQuantity()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
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
        BlueprintModel child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
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
        BlueprintModel blueprint = NewBlueprint("Frame");
        blueprint.Components.Add(NewComponent("Screw"), 1);

        BlueprintNode tree = BlueprintProcessor.BuildNode(blueprint, 4);

        tree.Name.Should().Be("Frame x4");
        tree.Crafts.Should().Be(4);
        tree.Children.Should().ContainSingle().Subject.Crafts.Should().Be(0); // components are gathered, not crafted
    }

    /// <summary>
    /// The craft count used to be appended to the label. Production time moved it into the per-step
    /// dialog, so the label carries the quantity alone and the count is read off the node.
    /// </summary>
    [Test]
    public void BuildNode_Yield_LabelsTheQuantityAndReportsCraftsSeparately()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);

        BlueprintNode three = BlueprintProcessor.BuildNode(blueprint, 3);
        three.Name.Should().Be("Bracket x3");
        three.Crafts.Should().Be(2);

        BlueprintNode two = BlueprintProcessor.BuildNode(blueprint, 2);
        two.Name.Should().Be("Bracket x2");
        two.Crafts.Should().Be(1);
    }

    [Test]
    public void BuildNode_Yield_ReportsWhatTheRoundedUpCraftsOverproduce()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);

        // Three needs two crafts, which make four.
        BlueprintProcessor.BuildNode(blueprint, 3).Surplus.Should().Be(1);
        BlueprintProcessor.BuildNode(blueprint, 4).Surplus.Should().Be(0);
    }

    [Test]
    public void BuildNode_ComponentLeaf_HasNoCraftsYieldOrSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);

        BlueprintNode leaf = BlueprintProcessor.BuildNode(blueprint, 1).Children.Should().ContainSingle().Subject;

        leaf.IsComponent.Should().BeTrue();
        leaf.Crafts.Should().Be(0);
        leaf.Yield.Should().Be(0);
        leaf.Surplus.Should().Be(0);
    }

    /// <summary>
    /// The step dialog shows yield beside surplus, because the surplus is only explicable as the
    /// remainder of rounding the quantity up to a whole number of crafts of that size.
    /// </summary>
    [Test]
    public void BuildNode_CarriesTheYieldThatExplainsTheSurplus()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 3);

        BlueprintNode node = BlueprintProcessor.BuildNode(blueprint, 4);

        node.Yield.Should().Be(3);
        node.Crafts.Should().Be(2);
        // Two crafts of three make six, so four leaves two spare.
        node.Surplus.Should().Be(2);
        (node.Crafts * node.Yield).Should().Be(node.Quantity + node.Surplus);
    }

    [Test]
    public void BuildNode_Yield_ScalesChildrenByCraftsRatherThanUnits()
    {
        BlueprintModel child = NewBlueprint("Bracket", yield: 2);
        child.Components.Add(NewComponent("Screw"), 3);

        BlueprintModel parent = NewBlueprint("Frame");
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
        BlueprintModel bracket = NewBlueprint("Bracket", yield: 2);

        BlueprintModel frame = NewBlueprint("Frame");
        frame.ChildBlueprints.Add(bracket, 2);

        BlueprintNode tree = BlueprintProcessor.BuildNode(frame, 1);

        // What CraftState.CraftingStepCount sums: one Frame craft plus one Bracket craft.
        tree.Crafts.Should().Be(1);
        tree.Children.Should().ContainSingle().Subject.Crafts.Should().Be(1);
    }

    [Test]
    public void BuildNode_HundredOfAYieldTwoBlueprint_IsFiftyCrafts()
    {
        BlueprintModel frame = NewBlueprint("Frame", yield: 2);

        BlueprintNode tree = BlueprintProcessor.BuildNode(frame, 100);

        tree.Quantity.Should().Be(100);
        tree.Crafts.Should().Be(50);
        tree.Name.Should().Be("Frame x100");
    }

    [Test]
    public void BuildNode_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        BlueprintModel blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.BuildNode(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void Flatten_ProductionTime_ScalesWithTheCraftsNeeded()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.ProductionTime = TimeSpan.FromSeconds(5);

        BlueprintProcessor.Flatten(blueprint, 3).ProductionTime.Should().Be(TimeSpan.FromSeconds(15));
    }

    /// <summary>
    /// The multiplier is the craft count, not the quantity: a yield of two halves the crafts, so it
    /// halves the time as well.
    /// </summary>
    [Test]
    public void Flatten_ProductionTime_FollowsCraftsRatherThanQuantity()
    {
        BlueprintModel blueprint = NewBlueprint("Bracket", yield: 2);
        blueprint.ProductionTime = TimeSpan.FromSeconds(10);

        BlueprintProcessor.Flatten(blueprint, 4).ProductionTime.Should().Be(TimeSpan.FromSeconds(20));
        // Three still takes two crafts, so it costs the same twenty seconds and leaves one spare.
        BlueprintProcessor.Flatten(blueprint, 3).ProductionTime.Should().Be(TimeSpan.FromSeconds(20));
    }

    [Test]
    public void Flatten_ProductionTime_SumsEveryDepthOfTheGraph()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.ProductionTime = TimeSpan.FromSeconds(2);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ProductionTime = TimeSpan.FromSeconds(5);
        parent.ChildBlueprints.Add(child, 3);

        // One Frame craft (5s) plus three Bracket crafts (6s).
        BlueprintProcessor.Flatten(parent, 1).ProductionTime.Should().Be(TimeSpan.FromSeconds(11));
    }

    /// <summary>
    /// Flatten reports blueprint craft time only. Component time depends on the merged component
    /// quantities, so BatchProcessor adds it where those are totalled.
    /// </summary>
    [Test]
    public void Flatten_ProductionTime_ExcludesComponentTime()
    {
        ComponentModel potato = NewComponent("Potato");
        potato.ProductionTime = TimeSpan.FromMinutes(5);

        BlueprintModel blueprint = NewBlueprint("Soup");
        blueprint.ProductionTime = TimeSpan.FromSeconds(30);
        blueprint.Components.Add(potato, 2);

        BlueprintProcessor.Flatten(blueprint, 1).ProductionTime.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Test]
    public void Flatten_NoProductionTimeAnywhere_IsZero()
    {
        BlueprintModel blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);

        BlueprintProcessor.Flatten(blueprint, 5).ProductionTime.Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void BuildNode_ProductionTime_IsTheRowsOwnTimeExcludingChildren()
    {
        BlueprintModel child = NewBlueprint("Bracket");
        child.ProductionTime = TimeSpan.FromSeconds(2);

        BlueprintModel parent = NewBlueprint("Frame");
        parent.ProductionTime = TimeSpan.FromSeconds(5);
        parent.ChildBlueprints.Add(child, 3);

        BlueprintNode tree = BlueprintProcessor.BuildNode(parent, 1);

        tree.ProductionTime.Should().Be(TimeSpan.FromSeconds(5));
        tree.Children.Should().ContainSingle().Subject.ProductionTime.Should().Be(TimeSpan.FromSeconds(6));
    }

    [Test]
    public void BuildNode_ComponentLeaf_ScalesProductionTimeByQuantity()
    {
        ComponentModel potato = NewComponent("Potato");
        potato.ProductionTime = TimeSpan.FromMinutes(5);

        BlueprintModel blueprint = NewBlueprint("Soup");
        blueprint.Components.Add(potato, 2);

        BlueprintNode leaf = BlueprintProcessor.BuildNode(blueprint, 3).Children.Should().ContainSingle().Subject;

        leaf.Quantity.Should().Be(6);
        leaf.ProductionTime.Should().Be(TimeSpan.FromMinutes(30));
    }
}
