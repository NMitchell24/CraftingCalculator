﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SmallLanternLight : Recipe
    {
        public SmallLanternLight()
        {
            Name = "Light (Small Lamp Style)";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.SODIUM, 5);
        }
    }
}
