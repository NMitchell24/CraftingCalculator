using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class FourierDelimiterRepair : ComplexRecipe
    {
        public FourierDelimiterRepair()
        {
            Name = "Fourier De-Limiter (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new SaltRefractor(), 1);
        }
    }
}
