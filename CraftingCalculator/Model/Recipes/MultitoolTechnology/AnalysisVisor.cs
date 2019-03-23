using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class AnalysisVisor : ComplexRecipe
    {
        public AnalysisVisor()
        {
            Name = "Analysis Visor";
            Type = RecipeFilterLabels.MultitoolTech;
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
