﻿using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class LifeSupportGel : ComplexRecipe
    {
        public LifeSupportGel()
        {
            Name = "Life Support Gel";
            Type = "Consumable";
            Ingredients.Add(IngredientType.CARBON, 20);
            ChildRecipes.Add(new DihydrogenJelly(), 1);
        }
    }
}
