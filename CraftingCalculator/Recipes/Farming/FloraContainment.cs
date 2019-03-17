using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class FloraContainment : Recipe
    {
        public FloraContainment()
        {
            Name = "Flora Containment";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CARBON, 15);
        }
    }
}
