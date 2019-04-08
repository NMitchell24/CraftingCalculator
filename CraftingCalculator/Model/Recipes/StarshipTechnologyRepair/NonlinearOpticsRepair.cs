using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class NonlinearOpticsRepair : ComplexRecipe
    {
        public NonlinearOpticsRepair()
        {
            Name = "Nonlinear Optics - Photon Cannon (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
