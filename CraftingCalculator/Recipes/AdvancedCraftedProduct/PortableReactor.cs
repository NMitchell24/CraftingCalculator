using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Recipes.AdvancedCraftedProduct
{
    class PortableReactor : ComplexRecipe
    {
        public PortableReactor()
        {
            Name = "Portable Reactor";
            Type = "Advanced Crafted Product";
            ChildRecipes.Add(new LiquidExplosive(), 1);
            ChildRecipes.Add(new FusionAccelerant(), 1);
        }
    }
}
