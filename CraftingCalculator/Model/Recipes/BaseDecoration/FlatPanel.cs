﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class FlatPanel : Recipe
    {
        public FlatPanel()
        {
            Name = "Flat Panel";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
