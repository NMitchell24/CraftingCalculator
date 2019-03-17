using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
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
