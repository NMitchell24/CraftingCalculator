using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.FreighterTechnology
{
    class FreighterWarpReactorSigma : ComplexRecipe
    {
        public FreighterWarpReactorSigma()
        {
            Name = "Freighter Warp Reactor Sigma";
            Type = "Freighter Warp Module";
            Ingredients.Add(IngredientType.CADMIUM, 250);
            ChildRecipes.Add(new OxygenFilter(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 4);
        }
    }
}
