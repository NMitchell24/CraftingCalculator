using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class LifeSupportModuleRepair : ComplexRecipe
    {
        public LifeSupportModuleRepair()
        {
            Name = "Life Support Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.OXYGEN, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
