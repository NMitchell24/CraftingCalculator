using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class BlueprintTests
{
    [Test]
    public void Tooltip_Setter_IsInertAndDoesNotRecurse()
    {
        Blueprint blueprint = new Blueprint { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = blueprint.Tooltip;

        blueprint.Tooltip = "anything";

        blueprint.Tooltip.Should().Be(computed);
    }

    [Test]
    public void Tooltip_Setter_IsInertThroughTheInterface()
    {
        IBaseDataRecord record = new Blueprint { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = record.Tooltip;

        record.Tooltip = "anything";

        record.Tooltip.Should().Be(computed);
    }
}
