using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class LargeHydroponicTray : Recipe
    {
        public LargeHydroponicTray()
        {
            Name = "Large Hydroponic Tray";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.PURE_FERRITE, 60);
            Ingredients.Add(IngredientType.IONISED_COBALT, 50);
            Ingredients.Add(IngredientType.CARBON, 50);
        }
    }
}
