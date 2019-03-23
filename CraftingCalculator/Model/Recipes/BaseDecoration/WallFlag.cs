using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class WallFlag : Recipe
    {
        public WallFlag()
        {
            Name = "Wall Flag (All)";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CARBON, 5);
        }
    }
}
