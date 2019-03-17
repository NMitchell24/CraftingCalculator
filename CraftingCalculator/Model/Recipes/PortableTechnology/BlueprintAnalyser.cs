using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
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
