using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ConcentratedDeposit
{
    class RareMetalElement : Recipe
    {
        public RareMetalElement()
        {
            Name = "Rare Metal Element";
            Type = RecipeFilterLabels.ConcentratedDeposits;
            Ingredients.Add(IngredientType.PURE_FERRITE, 150);
        }
    }
}
