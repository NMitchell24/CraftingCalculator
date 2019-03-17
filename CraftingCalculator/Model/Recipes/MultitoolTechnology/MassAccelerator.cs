using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class MassAccelerator : ComplexRecipe
    {
        public MassAccelerator()
        {
            Name = "Mass Accelerator";
            Type = "Blaze Javelin Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new SuperoxideCrystal(), 1);
        }
    }
}
