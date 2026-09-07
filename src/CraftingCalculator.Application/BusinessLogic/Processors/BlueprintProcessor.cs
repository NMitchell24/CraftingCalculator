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

    public static ComponentMap Flatten(Blueprint blueprint) => Flatten(blueprint, 0);

    private static ComponentMap Flatten(Blueprint blueprint, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        ComponentMap combined = new ComponentMap(blueprint.Components, false);

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            ComponentMap childComponents = Flatten(child.Blueprint, depth + 1);
            combined = ComponentProcessor.CombineComponents(childComponents, combined, child.Quantity);
        }

        return combined;
    }

    /// <summary>
    /// Builds the blueprint's component breakdown as a <see cref="BlueprintNode"/> tree, scaled by
    /// <paramref name="quantity"/>: one child node per component and per nested child blueprint,
    /// recursively.
    /// </summary>
    public static BlueprintNode BuildNode(Blueprint blueprint, long quantity) => BuildNode(blueprint, quantity, 0);

    private static BlueprintNode BuildNode(Blueprint blueprint, long quantity, int depth)
    {
        ThrowIfTooDeep(blueprint, depth);

        List<BlueprintNode> children = [];

        foreach (ComponentQuantity component in blueprint.Components.ComponentList)
        {
            long componentQuantity = component.Quantity * quantity;
            children.Add(new BlueprintNode(
                component.Name + " x" + componentQuantity, component.Name, component.Tooltip, true, componentQuantity, []));
        }

        foreach (BlueprintQuantity child in blueprint.ChildBlueprints.BlueprintList)
        {
            children.Add(BuildNode(child.Blueprint, child.Quantity * quantity, depth + 1));
        }

        return new BlueprintNode(
            blueprint.Name + " x" + quantity, blueprint.Name, blueprint.Tooltip, false, quantity, children);
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
