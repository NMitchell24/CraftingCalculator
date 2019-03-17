﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class SmallWedge : Recipe
    {
        public SmallWedge()
        {
            Name = "Small Wedge";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
