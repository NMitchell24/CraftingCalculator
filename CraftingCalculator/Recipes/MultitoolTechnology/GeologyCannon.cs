using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.MultitoolTechnology
{
    class GeologyCannon : ComplexRecipe
    {
        public GeologyCannon()
        {
            Name = "Geology Cannon";
            Type = "Multitool Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
            ChildRecipes.Add(new RareMetalElement(), 1);
        }
    }
}
