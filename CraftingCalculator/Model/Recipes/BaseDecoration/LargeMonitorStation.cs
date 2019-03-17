using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
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
