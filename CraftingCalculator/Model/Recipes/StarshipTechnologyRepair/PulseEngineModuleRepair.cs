using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class PulseEngineModuleRepair : Recipe
    {
        public PulseEngineModuleRepair()
        {
            Name = "Pulse Engine Module (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.DEUTERIUM, 75);
            Ingredients.Add(IngredientType.CADMIUM, 75);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
