﻿using CraftingCalculator.ViewModel.Ingredients;

namespace CraftingCalculator.Utilities
{
    public static class IngredientUtil
    {

        public static IngredientMap CombineIngredients(IngredientMap source, IngredientMap dest, int multiplier)
        {
            IngredientMap ret = new IngredientMap(dest);

            foreach (IngredientQuantity ingredient in source.IngredientList) 
            {
                ret.Add(ingredient.Name, ingredient.Description, (ingredient.Quantity * multiplier));
            }

            return ret;
        }
    }
}
