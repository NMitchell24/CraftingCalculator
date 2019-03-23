using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class LargeMonitorStation : Recipe
    {
        public LargeMonitorStation()
        {
            Name = "Large Monitor Station";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
        }
    }
}
