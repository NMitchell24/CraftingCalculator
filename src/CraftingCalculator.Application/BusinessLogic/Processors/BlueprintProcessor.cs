using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// The crafting rules for one blueprint: how many crafts a quantity takes under its yield, and how deep a
/// blueprint graph may nest before it is treated as a cycle.
/// </summary>
public static class BlueprintProcessor
{
    /// <summary>
    /// Recursion guard for the blueprint tree walks. Two graphs can drive a walk past the stack: one
    /// nested deeply enough, since nothing caps nesting depth at edit time, and one assembled in
    /// memory, where nothing has stopped a caller from nesting a blueprint inside itself. Bounding
    /// the depth turns either into a catchable exception instead of a process-killing
    /// StackOverflowException.
    /// </summary>
    internal const int MaxBlueprintDepth = 64;

    /// <summary>
    /// Whole crafts needed to produce <paramref name="quantity"/> items, rounded up: a craft is
    /// indivisible, so producing 3 of something that yields 2 takes 2 crafts and leaves 1 spare.
    /// A <paramref name="quantity"/> of 0 or less needs no crafts.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="yield"/> is less than 1.
    /// </exception>
    public static long CraftsFor(long quantity, long yield)
    {
        //Asserts the invariant rather than re-clamping: Blueprint.Yield already pins itself to 1, so
        //reaching here with less than that means a caller bypassed the model, and a silent clamp would
        //hide that behind a plausible-looking craft count.
        ArgumentOutOfRangeException.ThrowIfLessThan(yield, 1);

        //A quantity of 0 is a valid batch entry (see CraftState.SetQuantity), and the round-up
        //expression would report one craft for it rather than none.
        return quantity <= 0 ? 0 : (quantity - 1) / yield + 1;
    }

    /// <summary>
    /// Whether <paramref name="node"/> takes fewer crafts than the quantity it produces, because the
    /// blueprint yields more than one per craft. False for a component leaf, which is gathered rather
    /// than crafted, and false for a yield that happens to leave the two counts equal.
    /// </summary>
    public static bool CountsByCraft(BlueprintNode node) => !node.IsComponent && node.Crafts != node.Quantity;

    /// <summary>
    /// How many items one craft of <paramref name="blueprint"/> makes under <paramref name="settings"/>: its
    /// own yield, or 1 when the dataset does not use yield.
    /// </summary>
    internal static long YieldOf(BlueprintModel blueprint, Datasettings settings) => settings.UseYield ? blueprint.Yield : 1;

    /// <summary>
    /// Throws when <paramref name="depth"/>, the number of blueprints above <paramref name="blueprint"/> in a
    /// walk, is past <see cref="MaxBlueprintDepth"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The walk is nested too deeply.</exception>
    internal static void ThrowIfTooDeep(BlueprintModel blueprint, int depth)
    {
        if (depth > MaxBlueprintDepth)
        {
            // By id, not by name: this message reaches the log, which never carries anything the user typed.
            throw new InvalidOperationException(
                $"Blueprint graph exceeded the maximum depth of {MaxBlueprintDepth}; check for a cycle involving blueprint {blueprint.Id}.");
        }
    }
}
