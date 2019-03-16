using System.Collections.Generic;
using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes;

namespace CraftingCalculator.Utilities
{
    public static class IngredientUtil
    {
        public static IDictionary<IngredientType, int> GetIngredientsFor(Recipe recipe, int quantity)
        {
            IDictionary<IngredientType, int> ret = recipe.GetIngredients();

            foreach(KeyValuePair<IngredientType, int> ingredient in recipe.GetIngredients())
            {
                ret.Add(ingredient.Key, (ingredient.Value * quantity));
            }

            return ret;
        }

        public static IDictionary<IngredientType, int> CombineIngredients(IDictionary<IngredientType, int> source, 
            IDictionary<IngredientType, int> dest, int multiplier)
        {
            IDictionary<IngredientType, int> ret = dest;

            foreach(KeyValuePair<IngredientType, int> ingredient in source)
            {
                if (ret.ContainsKey(ingredient.Key))
                {
                    ret[ingredient.Key] = ret[ingredient.Key]
                        + (ingredient.Value * multiplier);
                }
                else
                {
                    ret.Add(ingredient.Key, (ingredient.Value * multiplier));
                }
            }

            return ret;
        }
    }
}
