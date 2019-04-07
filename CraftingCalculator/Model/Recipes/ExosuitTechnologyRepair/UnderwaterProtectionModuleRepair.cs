using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class UnderwaterProtectionModuleRepair : ComplexRecipe
    {
        public UnderwaterProtectionModuleRepair()
        {
            Name = "Underwater Protection Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new OxygenCapsule(), 1);
        }
    }
}
