using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnologyRepair
{
    class ExocraftAccelerationModuleRepair : Recipe
    {
        public ExocraftAccelerationModuleRepair()
        {
            Name = "Exocraft Acceleration Module (Repair)";
            Type = RecipeFilterLabels.ExocraftTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
