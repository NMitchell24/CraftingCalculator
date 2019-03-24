using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SweptSofa : Recipe
    {
        public SweptSofa()
        {
            Name = "Swept Sofa";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
