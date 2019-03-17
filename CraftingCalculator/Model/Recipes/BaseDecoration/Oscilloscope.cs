using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Oscilloscope : Recipe
    {
        public Oscilloscope()
        {
            Name = "Oscilloscope";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 10);
        }
    }
}
