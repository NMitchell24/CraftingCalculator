﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class BlazeJavelin : Recipe
    {
        public BlazeJavelin()
        {
            Name = "Blaze Javelin";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 200);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 150);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
