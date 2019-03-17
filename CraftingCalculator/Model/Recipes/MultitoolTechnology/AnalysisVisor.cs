using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class AnalysisVisor : ComplexRecipe
    {
        public AnalysisVisor()
        {
            Name = "Analysis Visor";
            Type = "Scanner Module";
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
