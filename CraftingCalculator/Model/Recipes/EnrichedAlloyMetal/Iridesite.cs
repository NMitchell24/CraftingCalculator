using CraftingCalculator.Model.Recipes.AlloyMetal;

namespace CraftingCalculator.Model.Recipes.EnrichedAlloyMetal
{
    class Iridesite : ComplexRecipe
    {
        public Iridesite()
        {
            Name = "Iridesite";
            Type = "Enriched Alloy Metal";
            ChildRecipes.Add(new Aronium(), 1);
            ChildRecipes.Add(new MagnoGold(), 1);
            ChildRecipes.Add(new Grantine(), 1);
        }
    }
}
