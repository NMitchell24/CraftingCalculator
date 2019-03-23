﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AlloyMetal
{
    class Lemmium : Recipe
    {
        public Lemmium()
        {
            Name = "Lemmium";
            Type = RecipeFilterLabels.AlloyMetal;
            Ingredients.Add(IngredientType.URANIUM, 50);
            Ingredients.Add(IngredientType.PURE_FERRITE, 100);
        }
    }
}
