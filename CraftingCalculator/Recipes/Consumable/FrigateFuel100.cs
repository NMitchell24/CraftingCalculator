using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Consumable
{
    class FrigateFuel100 : Recipe
    {
        public FrigateFuel100()
        {
            Name = "Frigate Fuel (100 Tonnes)";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.DIHYDROGEN, 100);
            Ingredients.Add(IngredientType.TRITIUM, 100);
        }
    }
}
