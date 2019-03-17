using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.FreighterTechnology
{
    class FreighterWarpReactorTheta : ComplexRecipe
    {
        public FreighterWarpReactorTheta()
        {
            Name = "Freighter Warp Reactor Theta";
            Type = "Freighter Warp Module";
            Ingredients.Add(IngredientType.INDIUM, 250);
            ChildRecipes.Add(new SaltRefractor(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 6);
        }
    }
}
