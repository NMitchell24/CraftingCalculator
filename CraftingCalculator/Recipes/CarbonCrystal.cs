using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class CarbonCrystal : Recipe
    {
        public CarbonCrystal()
        {
            Name = "Carbon Crystal";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 150);
        }
    }
}
