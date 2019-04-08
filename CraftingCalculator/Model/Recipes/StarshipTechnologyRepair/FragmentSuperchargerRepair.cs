using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class FragmentSuperchargerRepair : ComplexRecipe
    {
        public FragmentSuperchargerRepair()
        {
            Name = "Fragment Supercharger (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new RareMetalElement(), 1);
        }
    }
}
