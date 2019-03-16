using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes
{
    class CarbonNanotubes : Recipe
    {
        public CarbonNanotubes()
        {
            Name = "Carbon Nanotubes";
            Type = "Component";
            Ingredients.Add(IngredientType.CARBON, 50);
        }
    }
}
