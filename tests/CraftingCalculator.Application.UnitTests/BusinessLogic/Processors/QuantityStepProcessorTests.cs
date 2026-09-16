using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class QuantityStepProcessorTests
{
    [Test]
    public void Step_UpByOne_RaisesTheQuantity() =>
        QuantityStepProcessor.Step(3, 1).Should().Be(4);

    [Test]
    public void Step_DownByOne_LowersTheQuantity() =>
        QuantityStepProcessor.Step(3, -1).Should().Be(2);

    [Test]
    public void Step_DownByOneFromOne_SettlesAtZero() =>
        QuantityStepProcessor.Step(1, -1).Should().Be(0);

    [Test]
    public void Step_DownByTenFromFour_SettlesAtZero() =>
        QuantityStepProcessor.Step(4, -10).Should().Be(0);

    [Test]
    public void Step_DownByOneFromZero_RemovesTheEntry() =>
        QuantityStepProcessor.Step(0, -1).Should().BeNull();

    [Test]
    public void Step_DownByTenFromZero_RemovesTheEntry() =>
        QuantityStepProcessor.Step(0, -10).Should().BeNull();

    [Test]
    public void Step_UpPastMaxValue_SaturatesAtMaxValue() =>
        QuantityStepProcessor.Step(long.MaxValue - 3, 10).Should().Be(long.MaxValue);

    [Test]
    public void Step_ByZero_KeepsTheQuantity() =>
        QuantityStepProcessor.Step(5, 0).Should().Be(5);
}
