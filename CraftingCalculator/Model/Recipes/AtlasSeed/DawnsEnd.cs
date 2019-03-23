﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class DawnsEnd : ComplexRecipe
    {
        public DawnsEnd()
        {
            Name = "Dawn's End";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 25);
            ChildRecipes.Add(new DarkMatter(), 1);
        }
    }
}
