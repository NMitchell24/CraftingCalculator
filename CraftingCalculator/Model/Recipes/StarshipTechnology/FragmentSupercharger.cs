using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class FragmentSupercharger : ComplexRecipe
    {
        public FragmentSupercharger()
        {
            Name = "Fragment Supercharger";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new RareMetalElement(), 3);
        }
    }
}
