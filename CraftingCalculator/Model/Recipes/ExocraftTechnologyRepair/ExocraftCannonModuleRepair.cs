using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnologyRepair
{
    class ExocraftCannonModuleRepair : Recipe
    {
        public ExocraftCannonModuleRepair()
        {
            Name = "Exocraft Cannon Module (Repair)";
            Type = RecipeFilterLabels.ExocraftTechRepair;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
