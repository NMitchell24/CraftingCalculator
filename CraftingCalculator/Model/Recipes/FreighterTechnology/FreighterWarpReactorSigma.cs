using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.FreighterTechnology
{
    class FreighterWarpReactorSigma : ComplexRecipe
    {
        public FreighterWarpReactorSigma()
        {
            Name = "Freighter Warp Reactor Sigma";
            Type = RecipeFilterLabels.FreighterTech;
            Ingredients.Add(IngredientType.CADMIUM, 250);
            ChildRecipes.Add(new OxygenFilter(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 4);
        }
    }
}
