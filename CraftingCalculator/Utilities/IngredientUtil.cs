using CraftingCalculator.ViewModel.Ingredients;

namespace CraftingCalculator.Utilities
{
    public static class IngredientUtil
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
}
