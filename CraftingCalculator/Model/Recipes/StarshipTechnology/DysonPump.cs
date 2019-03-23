﻿using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class DysonPump : ComplexRecipe
    {
        public DysonPump()
        {
            Name = "Dyson Pump";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new TetraCobalt(), 3);
        }
    }
}
