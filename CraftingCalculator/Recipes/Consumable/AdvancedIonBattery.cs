using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Consumable
{
    class AdvancedIonBattery : Recipe
    {
        public AdvancedIonBattery()
        {
            Name = "Advanced Ion Battery";
            Type = "Consumable (Exosuit)";
            Ingredients.Add(IngredientType.IONISED_COBALT, 25);
            Ingredients.Add(IngredientType.PURE_FERRITE, 20);
        }
    }
}
