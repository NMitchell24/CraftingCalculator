using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class ToxicProtectionModuleRepair : Recipe
    {
        public ToxicProtectionModuleRepair()
        {
            Name = "Toxic Protection Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.AMMONIA, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
