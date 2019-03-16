using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class Lubricant : Recipe
    {
        public Lubricant()
        {
            Name = "Lubricant";
            Type = "Trade Item";
            Ingredients.Add(IngredientType.COPRITE, 50);
            Ingredients.Add(IngredientType.GAMMA_ROOT, 400);
        }
    }
}
