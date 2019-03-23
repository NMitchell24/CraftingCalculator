using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class SignalBooster : ComplexRecipe
    {
        public SignalBooster()
        {
            Name = "Signal Booster";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.SODIUM, 15);
            ChildRecipes.Add(new MetalPlating(), 1);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
