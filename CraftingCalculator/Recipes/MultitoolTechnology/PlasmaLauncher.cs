using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class PlasmaLauncher : ComplexRecipe
    {
        public PlasmaLauncher()
        {
            Name = "Plasma Launcher";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new SuperoxideCrystal(), 1);
        }
    }
}
