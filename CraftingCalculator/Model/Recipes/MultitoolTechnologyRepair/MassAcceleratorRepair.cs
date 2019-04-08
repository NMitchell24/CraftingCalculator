using CraftingCalculator.Model.Recipes.ConcentratedDeposit;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class MassAcceleratorRepair : ComplexRecipe
    {
        public MassAcceleratorRepair()
        {
            Name = "Mass Accelerator (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new SuperoxideCrystal(), 1);
        }
    }
}
