using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.ConcentratedDeposit;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class DysonPump : ComplexRecipe
    {
        public DysonPump()
        {
            Name = "Dyson Pump";
            Type = "Starship Cyclotron Ballista Module";
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 2);
            ChildRecipes.Add(new TetraCobalt(), 3);
        }
    }
}
