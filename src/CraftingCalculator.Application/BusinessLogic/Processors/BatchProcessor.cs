using CraftingCalculator.Domain.BusinessLogic;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked blueprints (each with its own quantity) into total cost, total value,
/// total production time, the merged raw materials needed to make all of them, and each one's
/// breakdown tree.
/// </summary>
public static class BatchProcessor
{
    /// <summary>
    /// The totals for <paramref name="batch"/>, worked out under <paramref name="settings"/>. Without
    /// <see cref="Datasettings.UseCosts"/> the total cost is 0; without <see cref="Datasettings.UseValues"/> the
    /// total value and the surplus value are 0. With <see cref="Datasettings.UseSharedSurplus"/>, a step uses what
    /// the steps before it overproduced before crafting any of its own, so the batch takes the fewest crafts it
    /// can and the surplus is only what is left at the end.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// A blueprint graph nests deeper than the app can work out, which a cycle does.
    /// </exception>
    public static BatchTotals CalculateTotals(IReadOnlyCollection<BlueprintQuantity> batch, Datasettings settings)
    {
        BatchWalk walk = new(settings);
        List<BlueprintNode> roots = new(batch.Count);
        double totalValue = 0;

        foreach (BlueprintQuantity blueprintQuantity in batch)
        {
            roots.Add(walk.Visit(blueprintQuantity.Blueprint, blueprintQuantity.Quantity, 0));

            //Value follows the quantity the batch asked for; what the rounding up overproduces is
            //reported through Surplus instead.
            if (settings.UseValues)
            {
                totalValue += blueprintQuantity.TotalValue;
            }
        }

        ComponentMap materials = walk.Materials;
        double totalCost = settings.UseCosts
            ? materials.ComponentList.Sum(componentQuantity => componentQuantity.TotalCost)
            : 0;
        //Component time is read off the merged materials instead of being accumulated during the walk:
        //it follows how many of each component the batch ends up needing, which is what that map already
        //holds. Components have no yield, so the multiplier is quantity rather than a craft count.
        TimeSpan componentTime = materials.ComponentList.Aggregate(
            TimeSpan.Zero, (total, componentQuantity) => DurationMath.Add(total, componentQuantity.TotalProductionTime));

        double surplusValue = settings.UseValues
            ? walk.Surplus.BlueprintList.Sum(blueprintQuantity => blueprintQuantity.TotalValue)
            : 0;

        return new BatchTotals(
            totalCost, totalValue, materials, walk.Surplus, surplusValue,
            DurationMath.Add(walk.BlueprintTime, componentTime), roots, walk.Crafts);
    }

    /// <summary>
    /// One pass over every tree in a batch, building each node and adding what it needs, overproduces and
    /// takes into totals shared by the whole batch. Trees are visited in batch order and each node's children in
    /// the order the Steps tree lists them, so a step only ever draws on surplus made by a step listed above it.
    /// </summary>
    private sealed class BatchWalk(Datasettings settings)
    {
        // The walk adds into maps of its own and never into blueprint.Components or blueprint.ChildBlueprints:
        // SnapshotModelProcessor shares one BlueprintModel across every tree it appears in, so writing into one
        // would change every other tree that blueprint is part of.
        public ComponentMap Materials { get; } = new();
        public BlueprintMap Surplus { get; } = new();
        public TimeSpan BlueprintTime { get; private set; }
        public long Crafts { get; private set; }

        /// <summary>
        /// Builds <paramref name="blueprint"/>'s breakdown tree for <paramref name="quantity"/> of it: one child
        /// node per component and per nested child blueprint, recursively. Children are scaled by the crafts
        /// this step takes, not by <paramref name="quantity"/> itself, so a yield above 1, or surplus an earlier
        /// step made, reduces everything below it.
        /// </summary>
        public BlueprintNode Visit(BlueprintModel blueprint, long quantity, int depth)
        {
            BlueprintProcessor.ThrowIfTooDeep(blueprint, depth);

            long yield = BlueprintProcessor.YieldOf(blueprint, settings);
            // Drawing from the surplus first is what keeps every blueprint's crafts at the minimum: a step only
            // crafts once the surplus is used up, so what is left over never reaches a whole craft's yield, and
            // the total comes out as the whole batch's demand rounded up once rather than at every position.
            long fromSurplus = settings.UseSharedSurplus ? Surplus.Take(blueprint, quantity) : 0;
            long toCraft = quantity - fromSurplus;
            long crafts = BlueprintProcessor.CraftsFor(toCraft, yield);
            long overproduced = crafts * yield - toCraft;
            if (overproduced > 0)
            {
                Surplus.Add(blueprint, overproduced);
            }

            TimeSpan productionTime = DurationMath.Scale(blueprint.ProductionTime, crafts);
            BlueprintTime = DurationMath.Add(BlueprintTime, productionTime);
            // Checked, so a batch whose step count passes long.MaxValue throws rather than showing a negative count.
            Crafts = checked(Crafts + crafts);

            // A step the surplus covers entirely isn't crafted. Walking its parts at a quantity of 0 would list
            // them under it, and add components the batch never uses to Materials at 0.
            IReadOnlyList<BlueprintNode> children =
                fromSurplus > 0 && crafts == 0 ? [] : VisitParts(blueprint, crafts, depth);

            return new BlueprintNode(
                blueprint,
                Quantity: quantity, Crafts: crafts, Yield: yield,
                Surplus: overproduced, FromSurplus: fromSurplus,
                ProductionTime: productionTime,
                Children: children);
        }

        /// <summary>
        /// One child node per component and per nested child blueprint of <paramref name="blueprint"/>, for
        /// <paramref name="crafts"/> crafts of it.
        /// </summary>
        private List<BlueprintNode> VisitParts(BlueprintModel blueprint, long crafts, int depth)
        {
            List<BlueprintNode> children =
                new(blueprint.Components.ComponentList.Count + blueprint.ChildBlueprints.BlueprintList.Count);

            foreach (ComponentQuantity component in blueprint.Components.ComponentList)
            {
                long componentQuantity = component.Quantity * crafts;
                Materials.Add(component.Component, componentQuantity);
                //Named from Quantity on: five adjacent long arguments would otherwise transpose silently.
                children.Add(new BlueprintNode(
                    component.Component,
                    Quantity: componentQuantity, Crafts: 0, Yield: 0, Surplus: 0, FromSurplus: 0,
                    ProductionTime: DurationMath.Scale(component.Component.ProductionTime, componentQuantity),
                    Children: []));
            }

            children.AddRange(blueprint.ChildBlueprints.BlueprintList.Select(
                child => Visit(child.Blueprint, child.Quantity * crafts, depth + 1)));

            return children;
        }
    }
}
