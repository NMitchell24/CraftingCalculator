﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class FrigateFuel50 : Recipe
    {
        public FrigateFuel50()
        {
            Name = "Frigate Fuel (50 Tonnes)";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.DIHYDROGEN, 50);
            Ingredients.Add(IngredientType.TRITIUM, 50);
        }
    }
}
