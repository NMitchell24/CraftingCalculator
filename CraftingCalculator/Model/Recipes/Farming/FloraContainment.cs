using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
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
