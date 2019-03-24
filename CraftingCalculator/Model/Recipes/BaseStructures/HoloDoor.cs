﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseStructures
{
    class HoloDoor : Recipe
    {
        public HoloDoor()
        {
            Name = "Holo Door";
            Type = RecipeFilterLabels.BaseStructures;
            Ingredients.Add(IngredientType.IONISED_COBALT, 40);
            Ingredients.Add(IngredientType.PURE_FERRITE, 40);
        }
    }
}
