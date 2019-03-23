﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseBasicComponentsConcrete
{
    class ConcreteRoofCorner : Recipe
    {
        public ConcreteRoofCorner()
        {
            Name = "Concrete Roof Corner";
            Type = RecipeFilterLabels.BaseComponentsConcrete;
            Ingredients.Add(IngredientType.FERRITE_DUST, 30);
        }
    }
}
