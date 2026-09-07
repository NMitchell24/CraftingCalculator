using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked blueprints (each with its own quantity) into total cost, total value, and
/// the merged raw materials needed to make all of them.
/// </summary>
public static class BatchProcessor
{
    public static (double TotalCost, double TotalValue, ComponentMap Materials) CalculateTotals(
        IReadOnlyCollection<BlueprintQuantity> batch)
    {
        ComponentMap materials = new();
        double totalValue = 0;

        foreach (BlueprintQuantity rq in batch)
        {
            materials = ComponentProcessor.CombineComponents(BlueprintProcessor.Flatten(rq.Blueprint), materials, rq.Quantity);
            totalValue += rq.TotalValue;
        }

        double totalCost = materials.ComponentList.Sum(i => i.TotalCost);

        return (totalCost, totalValue, materials);
    }
}
