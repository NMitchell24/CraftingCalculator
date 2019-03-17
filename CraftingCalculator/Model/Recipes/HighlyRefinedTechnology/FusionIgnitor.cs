using CraftingCalculator.Model.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Model.Recipes.EnrichedAlloyMetal;

namespace CraftingCalculator.Model.Recipes.HighlyRefinedTechnology
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
