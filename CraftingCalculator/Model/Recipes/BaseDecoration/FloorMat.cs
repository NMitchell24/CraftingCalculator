using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class FloorMat : Recipe
    {
        public FloorMat()
        {
            Name = "Floor Mat";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CARBON, 5);
        }
    }
}
