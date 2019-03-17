using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.FreighterTechnology
{
    class FreighterWarpReactorTau : ComplexRecipe
    {
        public FreighterWarpReactorTau()
        {
            Name = "Freighter Warp Reactor Tau";
            Type = "Freighter Warp Module";
            Ingredients.Add(IngredientType.EMERIL, 250);
            ChildRecipes.Add(new SodiumDiode(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 5);
        }
    }
}
