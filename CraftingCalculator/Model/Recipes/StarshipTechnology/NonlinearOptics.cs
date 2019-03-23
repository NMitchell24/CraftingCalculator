using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class NonlinearOptics : ComplexRecipe
    {
        public NonlinearOptics()
        {
            Name = "Nonlinear Optics";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
