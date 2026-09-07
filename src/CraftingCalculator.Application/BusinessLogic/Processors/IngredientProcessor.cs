using CraftingCalculator.Domain.Models;

namespace CraftingCalculator.Application.BusinessLogic.Processors;

public static class IngredientProcessor
{
    public static IngredientMap CombineIngredients(IngredientMap source, IngredientMap dest, long multiplier)
    {
        IngredientMap ret = new IngredientMap(dest, false);

        foreach (IngredientQuantity ingredient in source.IngredientList)
        {
            ret.Add(ingredient.Ingredient, (ingredient.Quantity * multiplier));
        }

        return ret;
    }
}
