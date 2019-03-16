using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class ShellGreaser : ComplexRecipe
    {
        public ShellGreaser()
        {
            Name = "Shell Greaser";
            Type = "Scatter Blaster Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new ChlorideLattice(), 1);
        }
    }
}
