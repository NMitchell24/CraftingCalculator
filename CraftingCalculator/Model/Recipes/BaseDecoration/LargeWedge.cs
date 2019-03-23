using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class LargeWedge : Recipe
    {
        public LargeWedge()
        {
            Name = "Large Wedge";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
