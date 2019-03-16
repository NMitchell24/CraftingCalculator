using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class SolarVine : Recipe
    {
        public SolarVine()
        {
            Name = "Solar Vine";
            Type = "Farming (Plantable Seed)";
            Ingredients.Add(IngredientType.SOLANIUM, 50);
            Ingredients.Add(IngredientType.PHOSPHORUS, 25);
        }
    }
}
