using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class PottedPlant : Recipe
    {
        public PottedPlant()
        {
            Name = "Potted Plant";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.CARBON, 10);
        }
    }
}
