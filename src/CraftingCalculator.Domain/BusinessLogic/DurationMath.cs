namespace CraftingCalculator.Domain.BusinessLogic;

/// <summary>
/// Whole-number arithmetic over production times, saturating at <see cref="TimeSpan.MaxValue"/> so an
/// absurd batch reports an absurd duration rather than a wrong one.
/// </summary>
/// <remarks>
/// Lives in Domain rather than alongside the processors in Application because
/// <see cref="Models.ComponentQuantity.TotalProductionTime"/> needs it and Domain depends on nothing.
/// </remarks>
public static class DurationMath
{
    /// <summary>
    /// <paramref name="duration"/> repeated <paramref name="count"/> times. A <paramref name="count"/>
    /// of 0 or less, or a <paramref name="duration"/> of <see cref="TimeSpan.Zero"/> or less, produces
    /// <see cref="TimeSpan.Zero"/>.
    /// </summary>
    public static TimeSpan Scale(TimeSpan duration, long count)
    {
        if (count <= 0 || duration <= TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        //Ticks rather than the TimeSpan * double operator: craft and item counts are whole numbers, so
        //integer math keeps the accumulation exact instead of routing it through floating point. The
        //multiplication is the operation that overflows, so it cannot also be the test - divide
        //MaxValue by one operand instead, which the guard above has established is positive.
        return count > long.MaxValue / duration.Ticks
            ? TimeSpan.MaxValue
            : TimeSpan.FromTicks(duration.Ticks * count);
    }

    /// <summary>
    /// <paramref name="left"/> plus <paramref name="right"/>.
    /// </summary>
    public static TimeSpan Add(TimeSpan left, TimeSpan right)
    {
        //TimeSpan's own + operator is checked and throws, which Scale's saturation makes reachable: two
        //saturated operands would take the whole batch calculation down. Both operands are non-negative
        //here - the models clamp ProductionTime to zero and Scale never returns less - so subtracting
        //one from MaxValue cannot itself overflow.
        return left.Ticks > TimeSpan.MaxValue.Ticks - right.Ticks
            ? TimeSpan.MaxValue
            : left + right;
    }
}
