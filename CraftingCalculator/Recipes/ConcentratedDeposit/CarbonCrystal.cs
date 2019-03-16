using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.ConcentratedDeposit
{
    class CarbonCrystal : Recipe
    {
        public CarbonCrystal()
        {
            Name = "Carbon Crystal";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 150);
        }
    }
}
