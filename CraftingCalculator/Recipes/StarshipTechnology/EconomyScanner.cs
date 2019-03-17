﻿using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class EconomyScanner : ComplexRecipe
    {
        public EconomyScanner()
        {
            Name = "Economy Scanner";
            Type = "Starship Scanner Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new Microprocessor(), 5);
        }
    }
}
