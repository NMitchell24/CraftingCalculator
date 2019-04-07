using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class HeatThermalProtectionModuleRepair : Recipe
    {
        public HeatThermalProtectionModuleRepair()
        {
            Name = "Thermal Protection Module - Heat (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.PHOSPHORUS, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
