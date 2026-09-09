using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class ComponentProcessorTests
{
    [Test]
    public void CombineComponents_AddsSourceQuantitiesOntoDestination()
    {
        ComponentModel component = new ComponentModel { Id = 1, Name = "Test", Description = "Test" };

        ComponentMap source = new ComponentMap();
        source.Add(component, 5);

        ComponentMap dest = new ComponentMap();
        dest.Add(component, 10);

        ComponentMap result = ComponentProcessor.CombineComponents(source, dest, 1);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Component.Name.Should().Be(component.Name);
        result.ComponentList[0].Quantity.Should().Be(15);
    }

    [Test]
    public void CombineComponents_MultipliesSourceQuantityBeforeAdding()
    {
        ComponentModel component = new ComponentModel { Id = 1, Name = "Test" };

        ComponentMap source = new ComponentMap();
        source.Add(component, 5);

        ComponentMap dest = new ComponentMap();

        ComponentMap result = ComponentProcessor.CombineComponents(source, dest, 3);

        result.ComponentList.Should().ContainSingle();
        result.ComponentList[0].Quantity.Should().Be(15);
    }
}
