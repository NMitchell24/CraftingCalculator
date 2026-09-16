namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// The one stepping rule every quantity stepper in the app follows.
/// </summary>
public static class QuantityStepProcessor
{
    /// <summary>
    /// The quantity after moving <paramref name="quantity"/> by <paramref name="step"/> (negative to step down), or
    /// <c>null</c> when the step removes the entry. Stepping down settles at zero; stepping down from zero is the
    /// removal; stepping up saturates at <see cref="long.MaxValue"/>.
    /// </summary>
    public static long? Step(long quantity, long step)
    {
        // Zero is the landing every step down passes through, so the step that starts there is a
        // deliberate second tap rather than an overshoot. That is what lets a step of any size clamp
        // without losing the remove gesture: -10 against a quantity of 4 settles on zero instead of
        // dropping the entry on one tap.
        if (step < 0 && quantity == 0)
        {
            return null;
        }

        // The addition is what overflows, so it cannot also be the test - compare against the headroom
        // left below MaxValue instead. Only a step up can overflow: quantity is never negative, so a
        // step down lands at worst a single step below zero, which Math.Max takes care of.
        long stepped = step > 0 && quantity > long.MaxValue - step
            ? long.MaxValue
            : quantity + step;

        return Math.Max(stepped, 0);
    }
}
