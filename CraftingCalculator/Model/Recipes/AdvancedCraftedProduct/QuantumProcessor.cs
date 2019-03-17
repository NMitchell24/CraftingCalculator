using CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct;
using CraftingCalculator.Model.Recipes.ManufacturedGasProduct;

namespace CraftingCalculator.Model.Recipes.AdvancedCraftedProduct
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
