﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class EmerilDrive : Recipe
    {
        public EmerilDrive()
        {
            Name = "Emeril Drive";
            Type = "Hyperdrive Module";
            Ingredients.Add(IngredientType.CADMIUM, 250);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
        }
    }
}
