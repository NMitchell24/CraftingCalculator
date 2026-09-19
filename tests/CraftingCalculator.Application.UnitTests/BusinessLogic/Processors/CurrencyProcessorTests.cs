using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using CraftingCalculator.Domain.Models;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
[SetCulture("en-US")]
public class CurrencyProcessorTests
{
    private static readonly Datasettings Gold = new(CurrencyLabel: "Gold");

    [TestCase(200, ExpectedResult = "$200.00")]
    [TestCase(12.5, ExpectedResult = "$12.50")]
    [TestCase(1234.567, ExpectedResult = "$1,234.57")]
    [TestCase(-40, ExpectedResult = "-$40.00")]
    public string Format_WithoutALabel_IsTheDevicesCurrency(double amount) =>
        CurrencyProcessor.Format(amount, Datasettings.Default);

    [TestCase(200, ExpectedResult = "200 Gold")]
    [TestCase(12.5, ExpectedResult = "12.5 Gold")]
    [TestCase(12.25, ExpectedResult = "12.25 Gold")]
    [TestCase(1234.567, ExpectedResult = "1,234.57 Gold")]
    [TestCase(-40, ExpectedResult = "-40 Gold")]
    [TestCase(0, ExpectedResult = "0 Gold")]
    public string Format_WithALabel_IsTheNumberAndTheLabel(double amount) => CurrencyProcessor.Format(amount, Gold);

    [Test]
    public void Format_WithALabel_DropsTheNoiseOfAddingUpDecimals()
    {
        CurrencyProcessor.Format(0.1 * 3 * 10, Gold).Should().Be("3 Gold");
    }

    // A value of 0.3 less a cost of 3 x 0.1, which comes out a hair below zero.
    [TestCase(null, ExpectedResult = "$0.00")]
    [TestCase("Gold", ExpectedResult = "0 Gold")]
    public string Format_AProfitThatShouldBeZero_IsZero(string? label) =>
        CurrencyProcessor.Format(0.3 - 0.1 * 3, new Datasettings(CurrencyLabel: label));

    [TestCase(0.125, ExpectedResult = "0.13 Gold")]
    [TestCase(-0.125, ExpectedResult = "-0.13 Gold")]
    public string Format_WithALabel_RoundsHalfCentsAwayFromZero(double amount) => CurrencyProcessor.Format(amount, Gold);

    [Test]
    [SetCulture("de-DE")]
    public void Format_WithALabel_GroupsDigitsTheDevicesWay()
    {
        CurrencyProcessor.Format(1234.5, Gold).Should().Be("1.234,5 Gold");
    }

    [TestCase(0.3 - 0.1 * 3, ExpectedResult = 0)]
    [TestCase(0.004, ExpectedResult = 0)]
    [TestCase(0.005, ExpectedResult = 0.01)]
    [TestCase(-0.005, ExpectedResult = -0.01)]
    [TestCase(12.5, ExpectedResult = 12.5)]
    public double Round_IsTheAmountToTheNearestHundredth(double amount) => CurrencyProcessor.Round(amount);

    [Test]
    public void Round_AnAmountJustBelowZero_IsNotNegative()
    {
        double.IsNegative(CurrencyProcessor.Round(0.3 - 0.1 * 3)).Should().BeFalse();
    }

    [TestCase(null, ExpectedResult = null)]
    [TestCase("", ExpectedResult = null)]
    [TestCase("   ", ExpectedResult = null)]
    [TestCase("Scrap", ExpectedResult = "Scrap")]
    [TestCase("  Galactic Credits ", ExpectedResult = "Galactic Credits")]
    public string? ToLabel_TrimsAndTurnsNothingIntoNull(string? text) => CurrencyProcessor.ToLabel(text);
}
