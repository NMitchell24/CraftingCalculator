﻿using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.ExocraftTechnology
{
    class ExocraftAccelerationModule : ComplexRecipe
    {
        public ExocraftAccelerationModule()
        {
            Name = "Exocraft Acceleration Module";
            Type = "Exocraft Acceleration Upgrade";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 15);
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 50);
            ChildRecipes.Add(new CarbonNanotubes(), 2);
        }
    }
}
