using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Consumable
{
    class FrigateFuel200 : Recipe
    {
        public FrigateFuel200()
        {
            Name = "Frigate Fuel (200 Tonnes)";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.DIHYDROGEN, 200);
            Ingredients.Add(IngredientType.TRITIUM, 200);
        }
    }
}
