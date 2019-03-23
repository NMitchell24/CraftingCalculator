using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class StandingPlanter : Recipe
    {
        public StandingPlanter()
        {
            Name = "Standing Planter";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
