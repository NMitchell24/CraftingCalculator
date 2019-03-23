using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class StandingLight : Recipe
    {
        public StandingLight()
        {
            Name = "Light (Standing Style)";
            Type = RecipeFilterLabels.BaseDecorations;
            Ingredients.Add(IngredientType.SODIUM, 5);
        }
    }
}
