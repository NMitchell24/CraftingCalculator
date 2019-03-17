using CraftingCalculator.Model.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Model.Recipes.ManufacturedGasProduct
{
    class Semiconductor : ComplexRecipe
    {
        public Semiconductor()
        {
            Name = "Semiconductor";
            Type = "Manufactured Gas Product";
            ChildRecipes.Add(new ThermicCondensate(), 1);
            ChildRecipes.Add(new NitrogenSalt(), 1);
        }
    }
}
