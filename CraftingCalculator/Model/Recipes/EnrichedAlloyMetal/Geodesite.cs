using CraftingCalculator.Model.Recipes.AlloyMetal;

namespace CraftingCalculator.Model.Recipes.EnrichedAlloyMetal
{
    class Geodesite : ComplexRecipe
    {
        public Geodesite()
        {
            Name = "Geodesite";
            Type = "Enriched Alloy Metal";
            ChildRecipes.Add(new DirtyBronze(), 1);
            ChildRecipes.Add(new Herox(), 1);
            ChildRecipes.Add(new Lemmium(), 1);
        }
    }
}
