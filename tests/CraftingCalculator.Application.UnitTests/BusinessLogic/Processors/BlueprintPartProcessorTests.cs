using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintPartProcessorTests
{
    private static Component NewComponent(int id, string name) => new Component { Id = id, Name = name };

    private static Blueprint NewBlueprint(int id, string name) => new Blueprint { Id = id, Name = name };

    [Test]
    public void GetComponents_ReturnsComponentsBeforeChildBlueprints_EachOrderedByName()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 4);
        blueprint.Components.Add(NewComponent(3, "Bolt"), 1);
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2);

        List<IBaseQuantityRecord> components = BlueprintPartProcessor.GetParts(blueprint);

        components.Select(c => c.Name).Should().Equal("Bolt", "Screw", "Bracket");
    }

    [Test]
    public void Add_NewComponent_AddsItToTheBlueprint()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");

        BlueprintPartProcessor.Add(blueprint, NewComponent(2, "Screw"), 3);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(3);
    }

    [Test]
    public void Add_ComponentAlreadyOnTheBlueprint_RaisesItsQuantity()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);

        BlueprintPartProcessor.Add(blueprint, NewComponent(2, "Screw"), 2);

        blueprint.Components.ComponentList.Should().ContainSingle()
            .Which.Quantity.Should().Be(5);
    }

    [Test]
    public void Add_ChildBlueprint_AddsItToTheBlueprint()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");

        BlueprintPartProcessor.Add(blueprint, NewBlueprint(4, "Bracket"), 2);

        blueprint.ChildBlueprints.BlueprintList.Should().ContainSingle()
            .Which.Quantity.Should().Be(2);
    }

    [Test]
    public void SetQuantity_PositiveQuantity_ReplacesTheQuantityRatherThanAddingToIt()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3);

        BlueprintPartProcessor.SetQuantity(blueprint, blueprint.Components.ComponentList[0], 7);

        blueprint.Components.ComponentList[0].Quantity.Should().Be(7);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheComponentAndRecordsItForTheDAO()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3, id: 11);

        BlueprintPartProcessor.SetQuantity(blueprint, blueprint.Components.ComponentList[0], 0);

        blueprint.Components.ComponentList.Should().BeEmpty();
        blueprint.Components.RemovedComponents.Should().ContainSingle().Which.Id.Should().Be(11);
    }

    [Test]
    public void SetQuantity_Zero_RemovesTheChildBlueprintAndRecordsItForTheDAO()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2, id: 12);

        BlueprintPartProcessor.SetQuantity(blueprint, blueprint.ChildBlueprints.BlueprintList[0], 0);

        blueprint.ChildBlueprints.BlueprintList.Should().BeEmpty();
        blueprint.ChildBlueprints.RemovedBlueprints.Should().ContainSingle().Which.Id.Should().Be(12);
    }

    [Test]
    public void Remove_ChildBlueprint_RemovesItAndRecordsItForTheDAO()
    {
        Blueprint blueprint = NewBlueprint(1, "Frame");
        blueprint.Components.Add(NewComponent(2, "Screw"), 3, id: 11);
        blueprint.ChildBlueprints.Add(NewBlueprint(4, "Bracket"), 2, id: 12);

        BlueprintPartProcessor.Remove(blueprint, blueprint.ChildBlueprints.BlueprintList[0]);

        blueprint.ChildBlueprints.BlueprintList.Should().BeEmpty();
        blueprint.ChildBlueprints.RemovedBlueprints.Should().ContainSingle().Which.Id.Should().Be(12);
        blueprint.Components.ComponentList.Should().ContainSingle();
    }
}
