using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class FlatPanel : Recipe
    {
        public FlatPanel()
        {
            Name = "Flat Panel";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.FERRITE_DUST, 10);
            Ingredients.Add(IngredientType.SODIUM, 10);
        }
    }
}
