using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

/// <summary>
/// Combines a batch of picked recipes (each with its own quantity) into total cost, total value, and
/// the merged raw materials needed to make all of them.
/// </summary>
public static class BatchProcessor
{
    public static (double TotalCost, double TotalValue, IngredientMap Materials) CalculateTotals(
        IReadOnlyCollection<RecipeQuantity> batch)
    {
        IngredientMap materials = new();
        double totalValue = 0;

        foreach (RecipeQuantity rq in batch)
        {
            materials = IngredientProcessor.CombineIngredients(RecipeProcessor.Flatten(rq.Recipe), materials, rq.Quantity);
            totalValue += rq.TotalValue;
        }

        double totalCost = materials.IngredientList.Sum(i => i.TotalCost);

        return (totalCost, totalValue, materials);
    }
}
