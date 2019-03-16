using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class RareMetalElement : Recipe
    {
        public RareMetalElement()
        {
            Name = "Rare Metal Element";
            Type = "Curiosity";
            Ingredients.Add(IngredientType.PURE_FERRITE, 150);
        }
    }
}
