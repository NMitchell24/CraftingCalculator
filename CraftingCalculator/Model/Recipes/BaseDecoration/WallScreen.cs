using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class WallScreen : Recipe
    {
        public WallScreen()
        {
            Name = "Wall Screen";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
