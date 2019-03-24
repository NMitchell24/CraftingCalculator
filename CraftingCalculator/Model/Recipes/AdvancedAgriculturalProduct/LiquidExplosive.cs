﻿
namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class LiquidExplosive : ComplexRecipe
    {
        public LiquidExplosive()
        {
            Name = "Liquid Explosive";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            ChildRecipes.Add(new Acid(), 1);
            ChildRecipes.Add(new UnstableGel(), 1);
        }
    }
}
