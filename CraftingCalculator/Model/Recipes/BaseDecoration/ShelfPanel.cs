using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class ShelfPanel : Recipe
    {
        public ShelfPanel()
        {
            Name = "Shelf Panel";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
