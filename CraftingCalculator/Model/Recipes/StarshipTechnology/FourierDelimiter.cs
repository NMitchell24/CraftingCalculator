using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class FourierDelimiter : ComplexRecipe
    {
        public FourierDelimiter()
        {
            Name = "Fourier De-limiter";
            Type = "Starship Phase Beam Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new SaltRefractor(), 1);
        }
    }
}
