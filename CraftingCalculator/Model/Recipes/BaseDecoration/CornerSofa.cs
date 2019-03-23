using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class CornerSofa : Recipe
    {
        public CornerSofa()
        {
            Name = "Corner Sofa";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
