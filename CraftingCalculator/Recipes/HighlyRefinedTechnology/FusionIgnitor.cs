using CraftingCalculator.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Recipes.EnrichedAlloyMetal;

namespace CraftingCalculator.Recipes.HighlyRefinedTechnology
{
    class FusionIgnitor : ComplexRecipe
    {
        public FusionIgnitor()
        {
            Name = "Fusion Ignitor";
            Type = "Highly Refined Technology";
            ChildRecipes.Add(new PortableReactor(), 1);
            ChildRecipes.Add(new QuantumProcessor(), 1);
            ChildRecipes.Add(new Geodesite(), 1);
        }
    }
}
