using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintPartProcessorTests
{
    private static ComponentModel NewComponent(int id, string name) =>
        new() { Id = id, Name = name };

    private static BlueprintModel NewBlueprint(int id, string name) =>
        new() { Id = id, Name = name };

    [Test]
    public void GetComponents_ReturnsComponentsBeforeChildBlueprints_EachOrderedByName()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);
        blueprint.Components.Add(NewComponent(3, "Bolt"), 1);
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2);

        List<IBaseQuantityRecord> components = BlueprintPartProcessor.GetParts(blueprint);

        components.Select(c => c.Name).Should().Equal("Bolt", "Screw", "Bracket");
    }

    [Test]
    public void FindPart_ComponentOnTheBlueprint_ReturnsItsPart()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);

        IBaseQuantityRecord? part = BlueprintPartProcessor.FindPart(blueprint, NewComponent(2, "Screw"));

        part.Should().BeSameAs(blueprint.Components.ComponentList[0]);
    }

    [Test]
    public void FindPart_ChildBlueprintSharingAComponentName_ReturnsTheChildBlueprint()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Axe");
        blueprint.Components.Add(NewComponent(2, "Bronze"), 1);
        blueprint.ChildBlueprints.Add(NewBlueprint(3, "Bronze"), 8);

        IBaseQuantityRecord? part = BlueprintPartProcessor.FindPart(blueprint, NewBlueprint(3, "Bronze"));

        part.Should().BeSameAs(blueprint.ChildBlueprints.BlueprintList[0]);
    }

    [Test]
    public void FindPart_ComponentSharingANameWithAnotherPart_ReturnsItsOwnPart()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);
        blueprint.Components.Add(NewComponent(3, "Screw"), 6);

        IBaseQuantityRecord? part = BlueprintPartProcessor.FindPart(blueprint, NewComponent(3, "Screw"));

        part.Should().BeSameAs(blueprint.Components.ComponentList[1]);
    }

    [Test]
    public void Remove_ComponentSharingANameWithAnotherPart_LeavesTheOtherInPlace()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);
        blueprint.Components.Add(NewComponent(3, "Screw"), 6);

        BlueprintPartProcessor.Remove(blueprint, blueprint.Components.ComponentList[0]);

        blueprint.Components.ComponentList.Should().ContainSingle().Which.Component.Id.Should().Be(3);
    }

    [Test]
    public void FindPart_RecordNotOnTheBlueprint_ReturnsNull()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);

        BlueprintPartProcessor.FindPart(blueprint, NewComponent(5, "Bolt")).Should().BeNull();
    }

    [Test]
    public void Add_NewComponent_AddsItToTheBlueprint()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");

        BlueprintPartProcessor.Add(blueprint, NewComponent(2, "Screw"), 3);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(3);
    }

    [Test]
    public void Add_ComponentAlreadyOnTheBlueprint_RaisesItsQuantity()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);

        BlueprintPartProcessor.Add(blueprint, NewComponent(2, "Screw"), 2);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(5);
    }

    [Test]
    public void Add_ChildBlueprint_AddsItToTheBlueprint()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");

        BlueprintPartProcessor.Add(blueprint, NewBlueprint(4, "Bracket"), 2);

        blueprint.ChildBlueprints.BlueprintList.Should().ContainSingle()
            .Which.Quantity.Should().Be(2);
    }

    [Test]
    public void Step_Up_RaisesTheQuantity()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);

        BlueprintPartProcessor.Step(blueprint, blueprint.Components.ComponentList[0], 1);

        blueprint.Components.ComponentList[0].Quantity.Should().Be(4);
    }

    [Test]
    public void Step_DownFromTwo_KeepsThePartAtOne()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 2);

        BlueprintPartProcessor.Step(blueprint, blueprint.Components.ComponentList[0], -1);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(1);
    }

    [Test]
    public void Step_DownFromOne_KeepsThePartAtZero()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 1);

        BlueprintPartProcessor.Step(blueprint, blueprint.Components.ComponentList[0], -1);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(0);
    }

    [Test]
    public void Step_DownByTenFromFour_SettlesAtZero()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);

        BlueprintPartProcessor.Step(blueprint, blueprint.Components.ComponentList[0], -10);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(0);
    }

    [Test]
    public void Step_DownFromZero_RemovesTheComponent()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);
        blueprint.Components.ComponentList[0].Quantity = 0;

        BlueprintPartProcessor.Step(blueprint, blueprint.Components.ComponentList[0], -1);

        blueprint.Components.ComponentList.Should().BeEmpty();
    }

    [Test]
    public void Step_DownFromZero_RemovesTheChildBlueprint()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2);
        blueprint.ChildBlueprints.BlueprintList[0].Quantity = 0;

        BlueprintPartProcessor.Step(blueprint, blueprint.ChildBlueprints.BlueprintList[0], -1);

        blueprint.ChildBlueprints.BlueprintList.Should().BeEmpty();
    }

    [Test]
    public void GetPartsAtZero_ReturnsOnlyThePartsWithAQuantityOfZero()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);
        blueprint.Components.Add(NewComponent(3, "Bolt"), 1);
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2);
        blueprint.Components.ComponentList.Single(component => component.Name == "Screw").Quantity = 0;
        blueprint.ChildBlueprints.BlueprintList[0].Quantity = 0;

        List<IBaseQuantityRecord> atZero = BlueprintPartProcessor.GetPartsAtZero(blueprint);

        atZero.Select(part => part.Name).Should().Equal("Screw", "Bracket");
    }

    [Test]
    public void Remove_ChildBlueprint_RemovesItAndLeavesTheComponents()
    {
        BlueprintModel blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2);

        BlueprintPartProcessor.Remove(blueprint, blueprint.ChildBlueprints.BlueprintList[0]);

        blueprint.ChildBlueprints.BlueprintList.Should().BeEmpty();
        blueprint.Components.ComponentList.Should().ContainSingle();
    }
}
