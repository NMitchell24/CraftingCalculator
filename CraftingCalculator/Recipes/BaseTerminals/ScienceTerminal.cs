﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseTerminals
{
    class ScienceTerminal : Recipe
    {
        public ScienceTerminal()
        {
            Name = "Science Terminal";
            Type = "Base Terminal";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 25);
        }
    }
}
