using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class DestabalisedSodium : Recipe
    {
        public DestabalisedSodium()
        {
            Name = "Destablised Sodium";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 150);
        }
    }
}
