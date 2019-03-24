using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class CeilingPanel : Recipe
    {
        public CeilingPanel()
        {
            Name = "Ceiling Panel";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
