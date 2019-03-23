using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class MonitorStation : Recipe
    {
        public MonitorStation()
        {
            Name = "Monitor Station";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
