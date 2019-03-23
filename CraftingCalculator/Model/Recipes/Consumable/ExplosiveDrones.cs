﻿using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class ExplosiveDrones : Recipe
    {
        public ExplosiveDrones()
        {
            Name = "Explosive Drones";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.WALKER_BRAIN, 1);
            Ingredients.Add(IngredientType.GOLD, 50);
        }
    }
}
