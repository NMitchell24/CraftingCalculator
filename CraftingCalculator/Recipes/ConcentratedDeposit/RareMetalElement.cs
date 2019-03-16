using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.ConcentratedDeposit
{
    class RareMetalElement : Recipe
    {
        public RareMetalElement()
        {
            Name = "Rare Metal Element";
            Type = "Concentrated Deposit";
            Ingredients.Add(IngredientType.PURE_FERRITE, 150);
        }
    }
}
