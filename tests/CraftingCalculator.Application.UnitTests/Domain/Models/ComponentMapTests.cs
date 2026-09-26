using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class ComponentMapTests
{
    [Test]
    public void Add_AComponentSharingAnEntrysName_AddsAnEntryOfItsOwn()
    {
        ComponentMap map = new();
        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 2);

        map.Add(new ComponentModel { Id = 2, Name = "Wood" }, 5);

        map.ComponentList.Select(entry => (entry.Component.Id, entry.Quantity)).Should().Equal((1, 2L), (2, 5L));
    }

    [Test]
    public void Add_TheSameComponentAgain_RaisesItsQuantity()
    {
        ComponentMap map = new();
        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 2);

        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 5);

        map.ComponentList.Should().ContainSingle().Which.Quantity.Should().Be(7);
    }

    [Test]
    public void RemoveAll_AComponentSharingAnEntrysName_RemovesOnlyItsOwnEntry()
    {
        ComponentMap map = new();
        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 2);
        map.Add(new ComponentModel { Id = 2, Name = "Wood" }, 5);

        map.RemoveAll(new ComponentModel { Id = 1, Name = "Wood" });

        map.ComponentList.Should().ContainSingle().Which.Component.Id.Should().Be(2);
    }

    [Test]
    public void Add_AComponentRemovedEarlier_AddsAFreshEntryAtTheEnd()
    {
        ComponentMap map = new();
        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 2);
        map.Add(new ComponentModel { Id = 2, Name = "Stone" }, 5);
        map.RemoveAll(new ComponentModel { Id = 1, Name = "Wood" });

        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 3);

        map.ComponentList.Select(entry => (entry.Component.Id, entry.Quantity)).Should().Equal((2, 5L), (1, 3L));
    }

    [Test]
    public void Add_AfterReset_StartsFromNothing()
    {
        ComponentMap map = new();
        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 2);
        map.Reset();

        map.Add(new ComponentModel { Id = 1, Name = "Wood" }, 3);

        map.ComponentList.Should().ContainSingle().Which.Quantity.Should().Be(3);
    }
}
