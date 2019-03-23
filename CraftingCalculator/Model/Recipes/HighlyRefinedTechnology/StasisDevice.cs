using CraftingCalculator.Model.Recipes.AdvancedCraftedProduct;
using CraftingCalculator.Model.Recipes.EnrichedAlloyMetal;

namespace CraftingCalculator.Model.Recipes.HighlyRefinedTechnology
{
    class StasisDevice : ComplexRecipe
    {
        public StasisDevice()
        {
            Name = "Stasis Device";
            Type = RecipeFilterLabels.HighlyRefinedTech;
            ChildRecipes.Add(new QuantumProcessor(), 1);
            ChildRecipes.Add(new CryogenicChamber(), 1);
            ChildRecipes.Add(new Iridesite(), 1);
        }
    }
}
