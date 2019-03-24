using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class FrigateFuel50 : Recipe
    {
        public FrigateFuel50()
        {
            Name = "Frigate Fuel (50 Tonnes)";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.DIHYDROGEN, 50);
            Ingredients.Add(IngredientType.TRITIUM, 50);
        }
    }
}
