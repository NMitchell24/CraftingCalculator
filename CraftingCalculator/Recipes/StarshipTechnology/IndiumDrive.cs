﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class IndiumDrive : Recipe
    {
        public IndiumDrive()
        {
            Name = "Indium Drive";
            Type = "Hyperdrive Module";
            Ingredients.Add(IngredientType.EMERIL, 250);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 3);
        }
    }
}
