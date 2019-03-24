using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class DestabalisedSodium : Recipe
    {
        public DestabalisedSodium()
        {
            Name = "Destablised Sodium";
            Type = RecipeFilterLabels.ConcentratedDeposits;
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 150);
        }
    }
}
