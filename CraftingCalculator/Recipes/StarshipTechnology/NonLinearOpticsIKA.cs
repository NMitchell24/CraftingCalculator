using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class NonLinearOpticsIKA : ComplexRecipe
    {
        public NonLinearOpticsIKA()
        {
            Name = "Non Linear Optics (Infra-Knife Accelerator)";
            Type = "Starship Infra-Knife Accelerator Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new DestabalisedSodium(), 3);
        }
    }
}
