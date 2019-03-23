﻿using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Model.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Model.Recipes.AdvancedCraftedProduct
{
    class PortableReactor : ComplexRecipe
    {
        public PortableReactor()
        {
            Name = "Portable Reactor";
            Type = RecipeFilterLabels.AdvancedCraftedProduct;
            ChildRecipes.Add(new LiquidExplosive(), 1);
            ChildRecipes.Add(new FusionAccelerant(), 1);
        }
    }
}
