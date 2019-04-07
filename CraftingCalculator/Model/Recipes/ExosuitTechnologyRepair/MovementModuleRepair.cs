using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExosuitTechnologyRepair
{
    class MovementModuleRepair : ComplexRecipe
    {
        public MovementModuleRepair()
        {
            Name = "Movement Module (Repair)";
            Type = RecipeFilterLabels.ExosuitTechRepair;
            Ingredients.Add(IngredientType.DIHYDROGEN, 50);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
