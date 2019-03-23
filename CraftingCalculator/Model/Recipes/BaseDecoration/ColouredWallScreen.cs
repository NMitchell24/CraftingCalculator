using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class ColouredWallScreen : Recipe
    {
        public ColouredWallScreen()
        {
            Name = "Wall Screen (Orange and Green)";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
