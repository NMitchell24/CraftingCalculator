using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class FrigateFuel200 : Recipe
    {
        public FrigateFuel200()
        {
            Name = "Frigate Fuel (200 Tonnes)";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.DIHYDROGEN, 200);
            Ingredients.Add(IngredientType.TRITIUM, 200);
        }
    }
}
