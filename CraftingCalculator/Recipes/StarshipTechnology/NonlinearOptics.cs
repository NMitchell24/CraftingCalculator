using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class NonlinearOptics : ComplexRecipe
    {
        public NonlinearOptics()
        {
            Name = "Nonlinear Optics";
            Type = "Starship Photon Cannon Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new CobaltMirror(), 1);
        }
    }
}
