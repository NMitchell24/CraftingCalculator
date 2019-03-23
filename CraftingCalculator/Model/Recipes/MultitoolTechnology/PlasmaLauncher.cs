using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class PlasmaLauncher : ComplexRecipe
    {
        public PlasmaLauncher()
        {
            Name = "Plasma Launcher";
            Type = RecipeFilterLabels.MultitoolTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new SuperoxideCrystal(), 1);
        }
    }
}
