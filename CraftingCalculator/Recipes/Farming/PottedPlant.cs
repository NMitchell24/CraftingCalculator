﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class PottedPlant : Recipe
    {
        public PottedPlant()
        {
            Name = "Potted Plant";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CARBON, 10);
        }
    }
}
