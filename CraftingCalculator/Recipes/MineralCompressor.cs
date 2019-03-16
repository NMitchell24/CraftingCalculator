using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class MineralCompressor : ComplexRecipe
    {
        public MineralCompressor()
        {
            Name = "Mineral Compressor";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.GOLD, 50);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
