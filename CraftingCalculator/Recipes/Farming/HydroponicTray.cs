using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class HydroponicTray : Recipe
    {
        public HydroponicTray()
        {
            Name = "Hydroponic Tray";
            Type = "Farming (Technology)";
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
