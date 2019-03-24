using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class CarbonCrystal : Recipe
    {
        public CarbonCrystal()
        {
            Name = "Carbon Crystal";
            Type = RecipeFilterLabels.ConcentratedDeposits;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 150);
        }
    }
}
