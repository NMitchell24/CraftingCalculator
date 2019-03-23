using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class FloraContainment : Recipe
    {
        public FloraContainment()
        {
            Name = "Flora Containment";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.CARBON, 15);
        }
    }
}
