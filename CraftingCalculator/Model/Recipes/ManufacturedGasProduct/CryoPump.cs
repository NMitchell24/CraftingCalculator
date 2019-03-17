using CraftingCalculator.Model.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Model.Recipes.ManufacturedGasProduct
{
    class CryoPump : ComplexRecipe
    {
        public CryoPump()
        {
            Name = "Cryo-Pump";
            Type = "Manufactured Gas Product";
            ChildRecipes.Add(new HotIce(), 1);
            ChildRecipes.Add(new ThermicCondensate(), 1);
        }
    }
}
