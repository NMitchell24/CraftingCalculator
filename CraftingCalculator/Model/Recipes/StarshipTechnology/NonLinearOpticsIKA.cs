using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class NonLinearOpticsIKA : ComplexRecipe
    {
        public NonLinearOpticsIKA()
        {
            Name = "Non Linear Optics - Infra-Knife Accelerator";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new DestabalisedSodium(), 3);
        }
    }
}
