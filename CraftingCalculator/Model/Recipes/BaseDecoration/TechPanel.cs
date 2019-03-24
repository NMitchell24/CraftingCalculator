using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class TechPanel : Recipe
    {
        public TechPanel()
        {
            Name = "Tech Panel";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
