using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class BlueprintModelTests
{
    [Test]
    public void Tooltip_Setter_IsInertAndDoesNotRecurse()
    {
        BlueprintModel blueprint = new BlueprintModel { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = blueprint.Tooltip;

        blueprint.Tooltip = "anything";

        blueprint.Tooltip.Should().Be(computed);
    }

    [Test]
    public void Tooltip_Setter_IsInertThroughTheInterface()
    {
        IBaseDataRecord record = new BlueprintModel { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = record.Tooltip;

        record.Tooltip = "anything";

        record.Tooltip.Should().Be(computed);
    }

    [Test]
    public void Yield_DefaultsToOne()
    {
        new BlueprintModel().Yield.Should().Be(1);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(-100)]
    public void Yield_BelowOne_ClampsToOne(long yield)
    {
        BlueprintModel blueprint = new BlueprintModel { Yield = yield };

        blueprint.Yield.Should().Be(1);
    }

    [Test]
    public void Clone_CarriesTheYield()
    {
        BlueprintModel blueprint = new BlueprintModel { Id = 1, Name = "Bracket", Yield = 4 };

        BlueprintModel clone = (BlueprintModel)blueprint.Clone();

        clone.Yield.Should().Be(4);
    }

    [Test]
    public void Tooltip_ShowsTheYieldOnlyWhenItIsAboveOne()
    {
        BlueprintModel single = new BlueprintModel { Id = 1, Name = "Frame" };
        BlueprintModel batched = new BlueprintModel { Id = 1, Name = "Bracket", Yield = 2 };

        single.Tooltip.Should().NotContain("Yield");
        batched.Tooltip.Should().Contain("Yield per Craft: 2");
    }
}
