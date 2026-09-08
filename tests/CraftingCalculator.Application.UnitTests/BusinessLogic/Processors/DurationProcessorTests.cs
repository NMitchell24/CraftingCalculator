using AwesomeAssertions;
using CraftingCalculator.Application.BusinessLogic.Processors;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.BusinessLogic.Processors;

[TestFixture]
public class DurationProcessorTests
{
    [Test]
    public void Format_Zero_IsInstant()
    {
        DurationProcessor.Format(TimeSpan.Zero).Should().Be("Instant");
    }

    /// <summary>
    /// Blueprint.ProductionTime and Component.ProductionTime both clamp a negative to zero, so this only
    /// covers a value that reached the formatter another way rather than a state the app can save.
    /// </summary>
    [Test]
    public void Format_Negative_IsInstant()
    {
        DurationProcessor.Format(TimeSpan.FromSeconds(-5)).Should().Be("Instant");
    }

    [TestCase(0.1, ExpectedResult = "0.1s")]
    [TestCase(5.5, ExpectedResult = "5.5s")]
    [TestCase(45, ExpectedResult = "45s")]
    [TestCase(59.9, ExpectedResult = "59.9s")]
    public string Format_UnderAMinute_IsSecondsWithTenths(double seconds) =>
        DurationProcessor.Format(TimeSpan.FromSeconds(seconds));

    [TestCase(60, ExpectedResult = "1:00")]
    [TestCase(90, ExpectedResult = "1:30")]
    [TestCase(300, ExpectedResult = "5:00")]
    [TestCase(3599, ExpectedResult = "59:59")]
    public string Format_UnderAnHour_IsMinutesAndSeconds(double seconds) =>
        DurationProcessor.Format(TimeSpan.FromSeconds(seconds));

    [TestCase(3600, ExpectedResult = "1:00:00")]
    [TestCase(7530, ExpectedResult = "2:05:30")]
    [TestCase(86399, ExpectedResult = "23:59:59")]
    public string Format_UnderADay_IsHoursMinutesAndSeconds(double seconds) =>
        DurationProcessor.Format(TimeSpan.FromSeconds(seconds));

    [Test]
    public void Format_ExactlyOneDay_UsesTheSingularDayLabel()
    {
        DurationProcessor.Format(TimeSpan.FromDays(1)).Should().Be("1 Day 00:00:00");
    }

    [Test]
    public void Format_SeveralDays_LeadsWithTheDayCount()
    {
        DurationProcessor.Format(new TimeSpan(3, 2, 4, 5)).Should().Be("3 Days 02:04:05");
    }

    /// <summary>
    /// Past a minute there is no slot for a fraction, so the tenths a user can enter are dropped rather
    /// than rounding the whole value up into the next second.
    /// </summary>
    [Test]
    public void Format_TenthsPastAMinute_AreDropped()
    {
        DurationProcessor.Format(TimeSpan.FromSeconds(90.5)).Should().Be("1:30");
    }
}
