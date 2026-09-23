using AwesomeAssertions;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.Models;

[TestFixture]
public class BlueprintModelTests
{
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
}
