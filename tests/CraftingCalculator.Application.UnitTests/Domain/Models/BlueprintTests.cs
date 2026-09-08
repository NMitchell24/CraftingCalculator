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

    [Test]
    public void Yield_DefaultsToOne()
    {
        new Blueprint().Yield.Should().Be(1);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    public void Yield_BelowOne_ClampsToOne(long yield)
    {
        Blueprint blueprint = new Blueprint { Yield = yield };

        blueprint.Yield.Should().Be(1);
    }

    [Test]
    public void Clone_CarriesTheYield()
    {
        Blueprint blueprint = new Blueprint { Id = 1, Name = "Bracket", Yield = 4 };

        Blueprint clone = (Blueprint)blueprint.Clone();

        clone.Yield.Should().Be(4);
    }

    [Test]
    public void Tooltip_ShowsTheYieldOnlyWhenItIsAboveOne()
    {
        Blueprint single = new Blueprint { Id = 1, Name = "Frame" };
        Blueprint batched = new Blueprint { Id = 1, Name = "Bracket", Yield = 2 };

        single.Tooltip.Should().NotContain("Yield");
        batched.Tooltip.Should().Contain("Yield per Craft: 2");
    }
}
