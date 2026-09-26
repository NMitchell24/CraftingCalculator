using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class BlueprintProcessorTests
{
    private static BlueprintNode BlueprintNodeOf(long quantity, long crafts) =>
        new(new BlueprintModel { Id = 1, Name = "Bracket" },
            Quantity: quantity, Crafts: crafts, Yield: 2, Surplus: 0, ProductionTime: TimeSpan.Zero, Children: []);

    [TestCase(0, 2, ExpectedResult = 0)]
    [TestCase(-5, 2, ExpectedResult = 0)]
    [TestCase(1, 1, ExpectedResult = 1)]
    [TestCase(4, 1, ExpectedResult = 4)]
    [TestCase(1, 2, ExpectedResult = 1)]
    [TestCase(2, 2, ExpectedResult = 1)]
    [TestCase(3, 2, ExpectedResult = 2)]
    [TestCase(4, 2, ExpectedResult = 2)]
    [TestCase(100, 2, ExpectedResult = 50)]
    [TestCase(10, 3, ExpectedResult = 4)]
    public long CraftsFor_RoundsUpToWholeCrafts(long quantity, long yield) =>
        BlueprintProcessor.CraftsFor(quantity, yield);

    [TestCase(0)]
    [TestCase(-1)]
    public void CraftsFor_YieldBelowOne_Throws(long yield)
    {
        Action act = () => BlueprintProcessor.CraftsFor(4, yield);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void CountsByCraft_FewerCraftsThanItems_IsTrue()
    {
        BlueprintProcessor.CountsByCraft(BlueprintNodeOf(quantity: 2, crafts: 1)).Should().BeTrue();
    }

    [Test]
    public void CountsByCraft_AsManyCraftsAsItems_IsFalse()
    {
        // One Bracket still takes one craft, so there is no second number to tell the user about.
        BlueprintProcessor.CountsByCraft(BlueprintNodeOf(quantity: 1, crafts: 1)).Should().BeFalse();
    }

    [Test]
    public void CountsByCraft_Component_IsFalse()
    {
        BlueprintNode leaf = new(new ComponentModel { Id = 1, Name = "Screw" },
            Quantity: 3, Crafts: 0, Yield: 0, Surplus: 0, ProductionTime: TimeSpan.Zero, Children: []);

        BlueprintProcessor.CountsByCraft(leaf).Should().BeFalse();
    }
}
