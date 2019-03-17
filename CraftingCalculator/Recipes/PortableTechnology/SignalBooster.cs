using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class SignalBooster : ComplexRecipe
    {
        public SignalBooster()
        {
            Name = "Signal Booster";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.SODIUM, 15);
            ChildRecipes.Add(new MetalPlating(), 1);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
