using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class DestabalisedSodium : Recipe
    {
        public DestabalisedSodium()
        {
            Name = "Destablised Sodium";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.SODIUM_NITRATE, 150);
        }
    }
}
