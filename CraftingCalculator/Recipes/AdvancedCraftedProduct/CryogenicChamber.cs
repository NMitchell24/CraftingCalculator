using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Recipes.AdvancedCraftedProduct
{
    class CryogenicChamber : ComplexRecipe
    {
        public CryogenicChamber()
        {
            Name = "Cryogenic Chamber";
            Type = "Advanced Crafted Product";
            ChildRecipes.Add(new LivingGlass(), 1);
            ChildRecipes.Add(new CryoPump(), 1);
        }
    }
}
