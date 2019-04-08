using CraftingCalculator.Model.Recipes.CraftingComponent;
using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.FreighterTechnologyRepair
{
    class FreighterWarpReactorSigmaRepair : ComplexRecipe
    {
        public FreighterWarpReactorSigmaRepair()
        {
            Name = "Freighter Warp Reactor Sigma (Repair)";
            Type = RecipeFilterLabels.FreighterTechRepair;
            Ingredients.Add(IngredientType.CADMIUM, 125);
            ChildRecipes.Add(new OxygenFilter(), 1);
            ChildRecipes.Add(new AntimatterHousing(), 2);
        }
    }
}
