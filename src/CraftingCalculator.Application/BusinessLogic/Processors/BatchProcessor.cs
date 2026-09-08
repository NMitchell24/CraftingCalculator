using CraftingCalculator.Domain.BusinessLogic;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked blueprints (each with its own quantity) into total cost, total value,
/// total production time, and the merged raw materials needed to make all of them.
/// </summary>
public static class BatchProcessor
{
    public static BatchTotals CalculateTotals(IReadOnlyCollection<BlueprintQuantity> batch)
    {
        ComponentMap materials = new();
        BlueprintMap surplus = new();
        double totalValue = 0;
        TimeSpan blueprintTime = TimeSpan.Zero;

        foreach (BlueprintQuantity blueprintQuantity in batch)
        {
            FlattenResult flattened =
                BlueprintProcessor.Flatten(blueprintQuantity.Blueprint, blueprintQuantity.Quantity);

            materials = ComponentProcessor.CombineComponents(flattened.Components, materials, 1);

            foreach (BlueprintQuantity spare in flattened.Surplus.BlueprintList)
            {
                surplus.Add(spare.Blueprint, spare.Quantity);
            }

            blueprintTime = DurationMath.Add(blueprintTime, flattened.ProductionTime);

            //Value follows the quantity the batch asked for; what the rounding up overproduces is
            //reported through Surplus instead.
            totalValue += blueprintQuantity.TotalValue;
        }

        double totalCost = materials.ComponentList.Sum(componentQuantity => componentQuantity.TotalCost);
        //Component time is read off the merged materials instead of being accumulated during the walk:
        //it follows how many of each component the batch ends up needing, which is what that map already
        //holds. Components have no yield, so the multiplier is quantity rather than a craft count.
        TimeSpan componentTime = materials.ComponentList.Aggregate(
            TimeSpan.Zero, (total, componentQuantity) => DurationMath.Add(total, componentQuantity.TotalProductionTime));

        return new BatchTotals(totalCost, totalValue, materials, surplus, DurationMath.Add(blueprintTime, componentTime));
    }
}
