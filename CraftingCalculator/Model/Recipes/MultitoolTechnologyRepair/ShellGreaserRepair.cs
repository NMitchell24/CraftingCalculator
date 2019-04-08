using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class ShellGreaserRepair : ComplexRecipe
    {
        public ShellGreaserRepair()
        {
            Name = "Shell Greaser (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new ChlorideLattice(), 1);
        }
    }
}
