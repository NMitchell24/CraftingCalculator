using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class HolographicAnalyser : ComplexRecipe
    {
        public HolographicAnalyser()
        {
            Name = "Holographic Analyser";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new OxygenFilter(), 1);
        }
    }
}
