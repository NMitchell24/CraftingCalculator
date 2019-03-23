﻿using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class ShellGreaser : ComplexRecipe
    {
        public ShellGreaser()
        {
            Name = "Shell Greaser";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new ChlorideLattice(), 1);
        }
    }
}
