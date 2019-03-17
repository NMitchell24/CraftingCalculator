using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.CraftingComponent
{
    class CarbonNanotubes : Recipe
    {
        public CarbonNanotubes()
        {
            Name = "Carbon Nanotubes";
            Type = "Crafting Component";
            Ingredients.Add(IngredientType.CARBON, 50);
        }
    }
}
