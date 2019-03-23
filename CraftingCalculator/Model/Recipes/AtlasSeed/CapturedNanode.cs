using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AtlasSeed
{
    class CapturedNanode : Recipe
    {
        public CapturedNanode()
        {
            Name = "Captured Nanode";
            Type = RecipeFilterLabels.AtlasSeed;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 100);
        }
    }
}
