using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class GammaWeed : Recipe
    {
        public GammaWeed()
        {
            Name = "Gamma Weed";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.GAMMA_ROOT, 50);
            Ingredients.Add(IngredientType.URANIUM, 25);
        }
    }
}
