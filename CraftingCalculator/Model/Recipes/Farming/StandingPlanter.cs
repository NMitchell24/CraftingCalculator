using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class StandingPlanter : Recipe
    {
        public StandingPlanter()
        {
            Name = "Standing Planter";
            Type = "Farming (Technology)";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
