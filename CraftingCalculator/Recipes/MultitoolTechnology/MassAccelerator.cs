using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.MultitoolTechnology
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
