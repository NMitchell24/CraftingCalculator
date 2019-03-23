using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class WallUnit : Recipe
    {
        public WallUnit()
        {
            Name = "Wall Unit";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
            Ingredients.Add(IngredientType.COBALT, 5);
        }
    }
}
