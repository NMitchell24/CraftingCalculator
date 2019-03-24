﻿using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
{
    class NautilonChamber : ComplexRecipe
    {
        public NautilonChamber()
        {
            Name = "Nautilon Chamber";
            Type = RecipeFilterLabels.ExocraftTerminals;
            Ingredients.Add(IngredientType.CRYSTAL_SULPHIDE, 4);
            Ingredients.Add(IngredientType.SALT, 100);
            ChildRecipes.Add(new MetalPlating(), 5);
        }
    }
}
