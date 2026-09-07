using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class RecipeTests
{
    [Test]
    public void Tooltip_Setter_IsInertAndDoesNotRecurse()
    {
        Recipe recipe = new Recipe { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = recipe.Tooltip;

        recipe.Tooltip = "anything";

        recipe.Tooltip.Should().Be(computed);
    }

    [Test]
    public void Tooltip_Setter_IsInertThroughTheInterface()
    {
        IBaseDataRecord record = new Recipe { Id = 1, Name = "Frame", Description = "A frame" };
        string computed = record.Tooltip;

        record.Tooltip = "anything";

        record.Tooltip.Should().Be(computed);
    }
}
