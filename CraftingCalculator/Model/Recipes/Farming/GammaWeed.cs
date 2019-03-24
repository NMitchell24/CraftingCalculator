using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class GammaWeed : Recipe
    {
        public GammaWeed()
        {
            Name = "Gamma Weed";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.GAMMA_ROOT, 50);
            Ingredients.Add(IngredientType.URANIUM, 25);
        }
    }
}
