using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnologyRepair
{
    class ExocraftFusionEngineModuleRepair : Recipe
    {
        public ExocraftFusionEngineModuleRepair()
        {
            Name = "Exocraft Fusion Engine Module (Repair)";
            Type = RecipeFilterLabels.ExocraftTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
