using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class ColdThermalProtectionModuleRepair : Recipe
    {
        public ColdThermalProtectionModuleRepair()
        {
            Name = "Thermal Protection Module - Cold (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.DIOXITE, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
