using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.Consumable
{
    class HolographicAnalyser : ComplexRecipe
    {
        public HolographicAnalyser()
        {
            Name = "Holographic Analyser";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new OxygenFilter(), 1);
        }
    }
}
