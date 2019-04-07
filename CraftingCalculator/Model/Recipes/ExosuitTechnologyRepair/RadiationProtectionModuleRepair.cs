using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class RadiationProtectionModuleRepair : Recipe
    {
        public RadiationProtectionModuleRepair()
        {
            Name = "Radiation Protection Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.URANIUM, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
