using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Bed : Recipe
    {
        public Bed()
        {
            Name = "Bed";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
