using CraftingCalculator.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Recipes.AdvancedCraftedProduct
{
    class QuantumProcessor : ComplexRecipe
    {
        public QuantumProcessor()
        {
            Name = "Quantum Processor";
            Type = "Advanced Crafted Product";
            ChildRecipes.Add(new CircuitBoard(), 1);
            ChildRecipes.Add(new Superconductor(), 1);
        }
    }
}
