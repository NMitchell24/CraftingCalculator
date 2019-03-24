using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Drawers : Recipe
    {
        public Drawers()
        {
            Name = "Drawers";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
