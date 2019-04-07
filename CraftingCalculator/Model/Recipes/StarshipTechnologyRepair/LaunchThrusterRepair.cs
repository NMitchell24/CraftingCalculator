using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class LaunchThrusterRepair : ComplexRecipe
    {
        public LaunchThrusterRepair()
        {
            Name = "Launch Thruster (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
            ChildRecipes.Add(new DihydrogenJelly(), 1);
        }
    }
}
