using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Pyramid : Recipe
    {
        public Pyramid()
        {
            Name = "Pyramid";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.FERRITE_DUST, 20);
        }
    }
}
