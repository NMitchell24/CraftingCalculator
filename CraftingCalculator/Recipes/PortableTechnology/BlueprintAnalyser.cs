using CraftingCalculator.Ingredients;
using CraftingCalculator.Recipes.CraftingComponent;

namespace CraftingCalculator.Recipes.PortableTechnology
{
    class BlueprintAnalyser : ComplexRecipe
    {
        public BlueprintAnalyser()
        {
            Name = "Blueprint Analyser";
            Type = "Portable Base Technology";
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 20);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
