using CraftingCalculator.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Recipes.EnrichedAlloyMetal;

namespace CraftingCalculator.Recipes.HighlyRefinedTechnology
{
    class StasisDevice : ComplexRecipe
    {
        public StasisDevice()
        {
            Name = "Stasis Device";
            Type = "Highly Refined Technology";
            ChildRecipes.Add(new QuantumProcessor(), 1);
            ChildRecipes.Add(new CryogenicChamber(), 1);
            ChildRecipes.Add(new Iridesite(), 1);
        }
    }
}
