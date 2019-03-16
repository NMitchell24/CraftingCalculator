using CraftingCalculator.Recipes.EnhancedGasProduct;

namespace CraftingCalculator.Recipes.ManufacturedGasProduct
{
    class Superconductor : ComplexRecipe
    {
        public Superconductor()
        {
            Name = "Superconductor";
            Type = "Manufactured Gas Product";
            ChildRecipes.Add(new Semiconductor(), 1);
            ChildRecipes.Add(new EnrichedCarbon(), 1);
        }
    }
}
