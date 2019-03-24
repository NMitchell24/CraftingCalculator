using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class SolarVine : Recipe
    {
        public SolarVine()
        {
            Name = "Solar Vine";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.SOLANIUM, 50);
            Ingredients.Add(IngredientType.PHOSPHORUS, 25);
        }
    }
}
