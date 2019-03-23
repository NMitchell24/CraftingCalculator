using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.Consumable
{
    class HydrothermalFuelCell : Recipe
    {
        public HydrothermalFuelCell()
        {
            Name = "Hydrothermal Fuel Cell";
            Type = RecipeFilterLabels.Consumables;
            Ingredients.Add(IngredientType.SALT, 40);
            Ingredients.Add(IngredientType.CYTOPHOSPHATE, 40);
            Ingredients.Add(IngredientType.CARBON, 40);
        }
    }
}
