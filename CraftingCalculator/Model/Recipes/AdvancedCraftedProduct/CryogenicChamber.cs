using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Model.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Model.Recipes.AdvancedCraftedProduct
{
    class CryogenicChamber : ComplexRecipe
    {
        public CryogenicChamber()
        {
            Name = "Cryogenic Chamber";
            Type = RecipeFilterLabels.AdvancedCraftedProduct;
            ChildRecipes.Add(new LivingGlass(), 1);
            ChildRecipes.Add(new CryoPump(), 1);
        }
    }
}
