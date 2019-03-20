using System.Collections.Generic;
using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes;

namespace CraftingCalculator.Utilities
{
    public static class IngredientUtil
    {

        public static IngredientMap CombineIngredients(IngredientMap source, IngredientMap dest, int multiplier)
        {
            IngredientMap ret = dest;

            foreach (IngredientQuantity ingredient in source.IngredientList) 
            {
                ret.Add(ingredient.Ingredient, (ingredient.Quantity * multiplier));
            }

            return ret;
        }
    }
}
