using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class FuelOxidiser : Recipe
    {
        public FuelOxidiser()
        {
            Name = "Fuel Oxidiser";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.QUAD_SERVO, 2);
            Ingredients.Add(IngredientType.GOLD, 50);
        }
    }
}
