using CraftingCalculator.Model.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Model.Recipes.ManufacturedGasProduct
{
    class FusionAccelerant : ComplexRecipe
    {
        public FusionAccelerant()
        {
            Name = "Fusion Accelerant";
            Type = "Manufactured Gas Product";
            ChildRecipes.Add(new OrganicCatalyst(), 1);
            ChildRecipes.Add(new NitrogenSalt(), 1);
        }
    }
}
