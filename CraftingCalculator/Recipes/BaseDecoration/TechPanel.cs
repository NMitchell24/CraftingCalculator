using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class TechPanel : Recipe
    {
        public TechPanel()
        {
            Name = "Tech Panel";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 5);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
