﻿using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.Consumable;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class LargeRocketTubes : ComplexRecipe
    {
        public LargeRocketTubes()
        {
            Name = "Large Rocket Tubes";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new UnstablePlasma(), 4);
        }
    }
}
