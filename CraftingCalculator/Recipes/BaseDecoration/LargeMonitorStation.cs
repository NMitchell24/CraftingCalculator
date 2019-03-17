using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class LargeMonitorStation : Recipe
    {
        public LargeMonitorStation()
        {
            Name = "Large Monitor Station";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
        }
    }
}
