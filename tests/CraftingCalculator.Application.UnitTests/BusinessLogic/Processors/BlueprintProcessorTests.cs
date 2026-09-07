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

    [Test]
    public void Flatten_SingleLevel_ReturnsOwnComponents()
    {
        Blueprint blueprint = NewBlueprint("Widget");
        blueprint.Components.Add(NewComponent("Screw"), 2);
        blueprint.Components.Add(NewComponent("Plate"), 1);

        ComponentMap result = BlueprintProcessor.Flatten(blueprint);

        result.ComponentList.Should().HaveCount(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Screw").Quantity.Should().Be(2);
        result.ComponentList.Single(componentQuantity => componentQuantity.Name == "Plate").Quantity.Should().Be(1);
    }

    [Test]
    public void Flatten_TwoLevels_ScalesChildComponentsByChildQuantity()
    {
        Blueprint child = NewBlueprint("Bracket");
        child.Components.Add(NewComponent("Screw"), 3);

        Blueprint parent = NewBlueprint("Frame");
        parent.ChildBlueprints.Add(child, 2);

        ComponentMap result = BlueprintProcessor.Flatten(parent);

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

        ComponentMap result = BlueprintProcessor.Flatten(parent);

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

        ComponentMap result = BlueprintProcessor.Flatten(parent);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Name.Should().Be("Bolt");
        result.ComponentList[0].Quantity.Should().Be(2);
    }

    [Test]
    public void Flatten_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Blueprint blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.Flatten(blueprint);

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
    public void BuildNode_Cycle_ThrowsInsteadOfOverflowingTheStack()
    {
        Blueprint blueprint = NewBlueprint("Self Referencing");
        blueprint.ChildBlueprints.Add(blueprint, 1);

        Action act = () => BlueprintProcessor.BuildNode(blueprint, 1);

        act.Should().Throw<InvalidOperationException>();
    }
}
