using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Sofa : Recipe
    {
        public Sofa()
        {
            Name = "Sofa";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
