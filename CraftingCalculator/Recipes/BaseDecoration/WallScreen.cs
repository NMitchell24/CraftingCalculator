﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class WallScreen : Recipe
    {
        public WallScreen()
        {
            Name = "Wall Screen";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
