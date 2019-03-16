using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AtlasSeed
{
    class CapturedNanode : Recipe
    {
        public CapturedNanode()
        {
            Name = "Captured Nanode";
            Type = "Atlas Seed";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
        }
    }
}
