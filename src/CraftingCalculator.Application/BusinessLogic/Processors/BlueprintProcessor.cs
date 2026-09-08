using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Flattens a blueprint's (already-loaded) component graph into its combined raw components, or into
/// a breakdown tree. Ports the recursive walk that used to live on the Blueprint model itself
/// (Blueprint.GetComponents / Blueprint.GetBlueprintNodes) now that models stay dumb.
/// </summary>
public static class BlueprintProcessor
{
    /// <summary>
    /// Recursion guard. The app does not otherwise detect a cycle in the blueprint graph (only
    /// self-reference is prevented at edit time - see the "no cycle guard" parity issue), so an
    /// A -> B -> A pair would otherwise recurse until the stack overflows. Bounding the depth turns
    /// that into a catchable exception instead of a process-killing StackOverflowException.
    /// </summary>
    public const int MaxBlueprintDepth = 64;

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
    /// Flattens the blueprint's own components and every nested child blueprint's components into one
    /// combined map for <paramref name="quantity"/> of the blueprint, alongside the items its rounded-up
    /// crafts produce beyond what was asked for.
    /// </summary>
    public static (ComponentMap Components, BlueprintMap Surplus) Flatten(Blueprint blueprint, long quantity)
    {
        BlueprintMap surplus = new();
        ComponentMap components = Flatten(blueprint, quantity, surplus, 0);

        return (components, surplus);
    }

    private static ComponentMap Flatten(Blueprint blueprint, long quantity, BlueprintMap surplus, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        long crafts = CraftsFor(quantity, blueprint.Yield);
        long overproduced = crafts * blueprint.Yield - quantity;
        if (overproduced > 0)
        {
            surplus.Add(blueprint, overproduced);
        }

        ComponentMap combined = ComponentProcessor.CombineComponents(blueprint.Components, new ComponentMap(), crafts);

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            ComponentMap childComponents = Flatten(child.Blueprint, child.Quantity * crafts, surplus, depth + 1);
            combined = ComponentProcessor.CombineComponents(childComponents, combined, 1);
        }

        return combined;
    }

    /// <summary>
    /// Builds the blueprint's component breakdown as a <see cref="BlueprintNode"/> tree, scaled by
    /// <paramref name="quantity"/>: one child node per component and per nested child blueprint,
    /// recursively. Children are scaled by the crafts <paramref name="quantity"/> takes, not by
    /// <paramref name="quantity"/> itself, so a yield above 1 reduces everything below it.
    /// </summary>
    public static BlueprintNode BuildNode(Blueprint blueprint, long quantity) => BuildNode(blueprint, quantity, 0);

    private static BlueprintNode BuildNode(Blueprint blueprint, long quantity, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        long crafts = CraftsFor(quantity, blueprint.Yield);
        List<BlueprintNode> children = [];

        foreach (ComponentQuantity component in blueprint.Components.ComponentList)
        {
            long componentQuantity = component.Quantity * crafts;
            children.Add(new BlueprintNode(
                component.Name + " x" + componentQuantity, component.Name, component.Tooltip, true, componentQuantity, 0, []));
        }

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            children.Add(BuildNode(child.Blueprint, child.Quantity * crafts, depth + 1));
        }

        return new BlueprintNode(
            NodeLabel(blueprint, quantity, crafts), blueprint.Name, blueprint.Tooltip, false, quantity, crafts, children);
    }

    private static string NodeLabel(Blueprint blueprint, long quantity, long crafts)
    {
        //Crafts never exceeds quantity, so the two differ only where the yield is above 1. At the
        //default yield the suffix would restate the number already in the label.
        if (crafts == quantity)
        {
            return blueprint.Name + " x" + quantity;
        }

        return blueprint.Name + " x" + quantity + " (" + crafts + (crafts == 1 ? " craft)" : " crafts)");
    }

    private static void ThrowIfTooDeep(Blueprint blueprint, int depth)
    {
        if (depth > MaxBlueprintDepth)
        {
            throw new InvalidOperationException(
                $"Blueprint graph exceeded the maximum depth of {MaxBlueprintDepth}; check for a cycle involving '{blueprint.Name}'.");
        }
    }
}
