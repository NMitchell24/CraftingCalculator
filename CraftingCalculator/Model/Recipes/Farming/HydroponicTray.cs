using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Farming
{
    class HydroponicTray : Recipe
    {
        public HydroponicTray()
        {
            Name = "Hydroponic Tray";
            Type = RecipeFilterLabels.Farming;
            Ingredients.Add(IngredientType.PURE_FERRITE, 30);
            Ingredients.Add(IngredientType.IONISED_COBALT, 20);
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
