﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class LShapedCorridor : Recipe
    {
        public LShapedCorridor()
        {
            Name = "L-Shaped Corridor";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
