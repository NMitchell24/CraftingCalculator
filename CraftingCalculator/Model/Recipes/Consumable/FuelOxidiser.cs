using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class FuelOxidiser : Recipe
    {
        public FuelOxidiser()
        {
            Name = "Fuel Oxidiser";
            Type = "Consumable (Frigate)";
            Ingredients.Add(IngredientType.QUAD_SERVO, 2);
            Ingredients.Add(IngredientType.GOLD, 50);
        }
    }
}
