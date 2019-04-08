using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class DysonPumpRepair : ComplexRecipe
    {
        public DysonPumpRepair()
        {
            Name = "Dyson Pump (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new TetraCobalt(), 2);
        }
    }
}
