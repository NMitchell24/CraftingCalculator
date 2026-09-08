using AwesomeAssertions;
using CraftingCalculator.Domain.BusinessLogic;
using NUnit.Framework;

namespace CraftingCalculator.Application.UnitTests.Domain.BusinessLogic;

[TestFixture]
public class DurationMathTests
{
    [Test]
    public void Scale_ByACount_MultipliesExactly()
    {
        DurationMath.Scale(TimeSpan.FromSeconds(5.5), 4).Should().Be(TimeSpan.FromSeconds(22));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Scale_ByZeroOrLess_IsZero(long count)
    {
        DurationMath.Scale(TimeSpan.FromHours(1), count).Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void Scale_AZeroDuration_IsZero()
    {
        DurationMath.Scale(TimeSpan.Zero, long.MaxValue).Should().Be(TimeSpan.Zero);
    }

    [Test]
    public void Scale_AtTheOverflowBoundary_StillMultiplies()
    {
        TimeSpan hour = TimeSpan.FromHours(1);
        long largestExactCount = long.MaxValue / hour.Ticks;

        DurationMath.Scale(hour, largestExactCount).Should().Be(TimeSpan.FromTicks(hour.Ticks * largestExactCount));
    }

    /// <summary>
    /// The multiplication is unchecked, so without the guard this wraps to a negative tick count, which
    /// DurationProcessor renders as "Instant" - a wrong answer that reads like a plausible one.
    /// </summary>
    [Test]
    public void Scale_PastTheOverflowBoundary_SaturatesRatherThanWrapping()
    {
        TimeSpan day = TimeSpan.FromHours(24);

        DurationMath.Scale(day, long.MaxValue / day.Ticks + 1).Should().Be(TimeSpan.MaxValue);
    }

    [Test]
    public void Add_TwoDurations_Sums()
    {
        DurationMath.Add(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30))
            .Should().Be(TimeSpan.FromSeconds(90));
    }

    /// <summary>
    /// TimeSpan's own + operator is checked and throws here, which Scale's saturation puts within reach
    /// of a batch total.
    /// </summary>
    [Test]
    public void Add_PastTheOverflowBoundary_SaturatesRatherThanThrowing()
    {
        DurationMath.Add(TimeSpan.MaxValue, TimeSpan.FromSeconds(1)).Should().Be(TimeSpan.MaxValue);
    }

    [Test]
    public void Add_AtTheOverflowBoundary_StillSums()
    {
        TimeSpan almostMax = TimeSpan.MaxValue - TimeSpan.FromTicks(1);

        DurationMath.Add(almostMax, TimeSpan.FromTicks(1)).Should().Be(TimeSpan.MaxValue);
    }
}
