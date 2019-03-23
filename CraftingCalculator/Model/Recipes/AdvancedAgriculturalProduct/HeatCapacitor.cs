﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class HeatCapacitor : Recipe
    {
        public HeatCapacitor()
        {
            Name = "Heat Capacitor";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 100);
            Ingredients.Add(IngredientType.SOLANIUM, 200);
        }
    }
}
