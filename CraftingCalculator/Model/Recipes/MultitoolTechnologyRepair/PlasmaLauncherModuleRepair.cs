using CraftingCalculator.Model.Recipes.Consumable;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class PlasmaLauncherModuleRepair : ComplexRecipe
    {
        public PlasmaLauncherModuleRepair()
        {
            Name = "Plasma Launcher Module (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new UnstablePlasma(), 1);
        }
    }
}
