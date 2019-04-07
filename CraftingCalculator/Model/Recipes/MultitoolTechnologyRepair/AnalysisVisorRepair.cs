using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class AnalysisVisorRepair : ComplexRecipe
    {
        public AnalysisVisorRepair()
        {
            Name = "Analysis Visor (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            ChildRecipes.Add(new CarbonNanotubes(), 1);
        }
    }
}
