using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnologyRepair
{
    class ExocraftMiningLaserModuleRepair : Recipe
    {
        public ExocraftMiningLaserModuleRepair()
        {
            Name = "Exocraft Mining Laser Module (Repair)";
            Type = RecipeFilterLabels.ExocraftTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
