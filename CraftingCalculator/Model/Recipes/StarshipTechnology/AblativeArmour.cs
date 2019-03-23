﻿using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class AblativeArmour : ComplexRecipe
    {
        public AblativeArmour()
        {
            Name = "Ablative Armour";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 50);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
