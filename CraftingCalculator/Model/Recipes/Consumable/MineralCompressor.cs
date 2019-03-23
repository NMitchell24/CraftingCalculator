using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class MineralCompressor : ComplexRecipe
    {
        public MineralCompressor()
        {
            Name = "Mineral Compressor";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
