﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class CornerSofa : Recipe
    {
        public CornerSofa()
        {
            Name = "Corner Sofa";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
