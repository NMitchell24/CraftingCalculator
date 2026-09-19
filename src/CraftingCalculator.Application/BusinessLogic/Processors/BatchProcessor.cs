using CraftingCalculator.Domain.BusinessLogic;
using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked blueprints (each with its own quantity) into total cost, total value,
/// total production time, and the merged raw materials needed to make all of them.
/// </summary>
public static class BatchProcessor
{
    /// <summary>
    /// The totals for <paramref name="batch"/>, worked out under <paramref name="settings"/>. Without
    /// <see cref="Datasettings.UseCosts"/> the total cost is 0; without <see cref="Datasettings.UseValues"/> the
    /// total value and the surplus value are 0.
    /// </summary>
    public static BatchTotals CalculateTotals(IReadOnlyCollection<BlueprintQuantity> batch, Datasettings settings)
    {
        ComponentMap materials = new();
        BlueprintMap surplus = new();
        double totalValue = 0;
        TimeSpan blueprintTime = TimeSpan.Zero;

        foreach (BlueprintQuantity blueprintQuantity in batch)
        {
            FlattenResult flattened =
                BlueprintProcessor.Flatten(blueprintQuantity.Blueprint, blueprintQuantity.Quantity, settings);

            materials = ComponentProcessor.CombineComponents(flattened.Components, materials, 1);

            foreach (BlueprintQuantity spare in flattened.Surplus.BlueprintList)
            {
                surplus.Add(spare.Blueprint, spare.Quantity);
            }

            blueprintTime = DurationMath.Add(blueprintTime, flattened.ProductionTime);

            //Value follows the quantity the batch asked for; what the rounding up overproduces is
            //reported through Surplus instead.
            if (settings.UseValues)
            {
                totalValue += blueprintQuantity.TotalValue;
            }
        }

        double totalCost = settings.UseCosts
            ? materials.ComponentList.Sum(componentQuantity => componentQuantity.TotalCost)
            : 0;
        //Component time is read off the merged materials instead of being accumulated during the walk:
        //it follows how many of each component the batch ends up needing, which is what that map already
        //holds. Components have no yield, so the multiplier is quantity rather than a craft count.
        TimeSpan componentTime = materials.ComponentList.Aggregate(
            TimeSpan.Zero, (total, componentQuantity) => DurationMath.Add(total, componentQuantity.TotalProductionTime));

        double surplusValue = settings.UseValues
            ? surplus.BlueprintList.Sum(blueprintQuantity => blueprintQuantity.TotalValue)
            : 0;

        return new BatchTotals(
            totalCost, totalValue, materials, surplus, surplusValue, DurationMath.Add(blueprintTime, componentTime));
    }
}
