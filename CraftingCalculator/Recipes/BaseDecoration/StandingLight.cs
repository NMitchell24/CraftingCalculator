using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class StandingLight : Recipe
    {
        public StandingLight()
        {
            Name = "Light (Standing Style)";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.SODIUM, 5);
        }
    }
}
