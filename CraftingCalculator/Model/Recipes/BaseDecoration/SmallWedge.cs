using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class SmallWedge : Recipe
    {
        public SmallWedge()
        {
            Name = "Small Wedge";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
