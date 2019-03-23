using CraftingCalculator.Model.Ingredients;
using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class BlueprintAnalyser : ComplexRecipe
    {
        public BlueprintAnalyser()
        {
            Name = "Blueprint Analyser";
            Type = RecipeFilterLabels.PortableTech;
            Ingredients.Add(IngredientType.MAGNETISED_FERRITE, 20);
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
