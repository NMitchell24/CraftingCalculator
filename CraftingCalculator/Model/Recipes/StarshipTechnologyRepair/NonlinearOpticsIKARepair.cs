using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class NonlinearOpticsIKARepair : ComplexRecipe
    {
        public NonlinearOpticsIKARepair()
        {
            Name = "Nonlinear Optics - Infra-Knife Accelerator (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new DestabalisedSodium(), 2);
        }
    }
}
