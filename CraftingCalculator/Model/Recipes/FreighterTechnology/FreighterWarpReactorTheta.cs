using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.FreighterTechnology
{
    class FreighterWarpReactorTheta : ComplexRecipe
    {
        public FreighterWarpReactorTheta()
        {
            Name = "Freighter Warp Reactor Theta";
            Type = RecipeFilterLabels.FreighterTech;
            Ingredients.Add(IngredientType.INDIUM, 250);
            ChildRecipes.Add(new SaltRefractor(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 6);
        }
    }
}
