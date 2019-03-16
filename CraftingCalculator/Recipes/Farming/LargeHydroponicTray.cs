using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.Farming
{
    class LargeHydroponicTray : Recipe
    {
        public LargeHydroponicTray()
        {
            Name = "Large Hydroponic Tray";
            Type = "Farming (Technology)";
            Ingredients.Add(IngredientType.PURE_FERRITE, 60);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
            Ingredients.Add(IngredientType.CARBON, 50);
        }
    }
}
