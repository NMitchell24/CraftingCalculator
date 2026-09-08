using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked blueprints (each with its own quantity) into total cost, total value, and
/// the merged raw materials needed to make all of them.
/// </summary>
public static class BatchProcessor
{
    public static BatchTotals CalculateTotals(IReadOnlyCollection<BlueprintQuantity> batch)
    {
        ComponentMap materials = new();
        BlueprintMap surplus = new();
        double totalValue = 0;

        foreach (BlueprintQuantity blueprintQuantity in batch)
        {
            (ComponentMap components, BlueprintMap blueprintSurplus) =
                BlueprintProcessor.Flatten(blueprintQuantity.Blueprint, blueprintQuantity.Quantity);

            materials = ComponentProcessor.CombineComponents(components, materials, 1);

            foreach (BlueprintQuantity spare in blueprintSurplus.BlueprintList)
            {
                surplus.Add(spare.Blueprint, spare.Quantity);
            }

            //Value follows the quantity the batch asked for; what the rounding up overproduces is
            //reported through Surplus instead.
            totalValue += blueprintQuantity.TotalValue;
        }

        double totalCost = materials.ComponentList.Sum(componentQuantity => componentQuantity.TotalCost);

        return new BatchTotals(totalCost, totalValue, materials, surplus);
    }
}
