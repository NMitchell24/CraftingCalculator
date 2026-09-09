using CraftingCalculator.Domain.BusinessLogic;
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
    /// crafts produce beyond what was asked for and how long those crafts take.
    /// </summary>
    public static FlattenResult Flatten(BlueprintModel blueprint, long quantity)
    {
        BlueprintMap surplus = new();
        (ComponentMap components, TimeSpan productionTime) = Flatten(blueprint, quantity, surplus, 0);

        return new FlattenResult(components, surplus, productionTime);
    }

    private static (ComponentMap Components, TimeSpan ProductionTime) Flatten(
        BlueprintModel blueprint, long quantity, BlueprintMap surplus, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        long crafts = CraftsFor(quantity, blueprint.Yield);
        long overproduced = crafts * blueprint.Yield - quantity;
        if (overproduced > 0)
        {
            surplus.Add(blueprint, overproduced);
        }

        ComponentMap combined = ComponentProcessor.CombineComponents(blueprint.Components, new ComponentMap(), crafts);
        TimeSpan productionTime = DurationMath.Scale(blueprint.ProductionTime, crafts);

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            (ComponentMap childComponents, TimeSpan childTime) =
                Flatten(child.Blueprint, child.Quantity * crafts, surplus, depth + 1);
            combined = ComponentProcessor.CombineComponents(childComponents, combined, 1);
            productionTime = DurationMath.Add(productionTime, childTime);
        }

        return (combined, productionTime);
    }

    /// <summary>
    /// Builds the blueprint's component breakdown as a <see cref="BlueprintNode"/> tree, scaled by
    /// <paramref name="quantity"/>: one child node per component and per nested child blueprint,
    /// recursively. Children are scaled by the crafts <paramref name="quantity"/> takes, not by
    /// <paramref name="quantity"/> itself, so a yield above 1 reduces everything below it.
    /// </summary>
    public static BlueprintNode BuildNode(BlueprintModel blueprint, long quantity) => BuildNode(blueprint, quantity, 0);

    private static BlueprintNode BuildNode(BlueprintModel blueprint, long quantity, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        long crafts = CraftsFor(quantity, blueprint.Yield);
        List<BlueprintNode> children = [];

        foreach (ComponentQuantity component in blueprint.Components.ComponentList)
        {
            long componentQuantity = component.Quantity * crafts;
            //Named from Quantity on: four adjacent long arguments would otherwise transpose silently.
            children.Add(new BlueprintNode(
                component.Name, component.Tooltip, IsComponent: true,
                Quantity: componentQuantity, Crafts: 0, Yield: 0, Surplus: 0,
                ProductionTime: DurationMath.Scale(component.Component.ProductionTime, componentQuantity),
                Children: []));
        }

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            children.Add(BuildNode(child.Blueprint, child.Quantity * crafts, depth + 1));
        }

        return new BlueprintNode(
            blueprint.Name ?? "", blueprint.Tooltip, IsComponent: false,
            Quantity: quantity, Crafts: crafts, Yield: blueprint.Yield,
            Surplus: crafts * blueprint.Yield - quantity,
            ProductionTime: DurationMath.Scale(blueprint.ProductionTime, crafts),
            Children: children);
    }

    /// <summary>
    /// Whether <paramref name="node"/> takes fewer crafts than the quantity it produces, because the
    /// blueprint yields more than one per craft. False for a component leaf, which is gathered rather
    /// than crafted, and false for a yield that happens to leave the two counts equal.
    /// </summary>
    public static bool CountsByCraft(BlueprintNode node) => !node.IsComponent && node.Crafts != node.Quantity;

    private static void ThrowIfTooDeep(BlueprintModel blueprint, int depth)
    {
        if (depth > MaxBlueprintDepth)
        {
            throw new InvalidOperationException(
                $"Blueprint graph exceeded the maximum depth of {MaxBlueprintDepth}; check for a cycle involving '{blueprint.Name}'.");
        }
    }
}
