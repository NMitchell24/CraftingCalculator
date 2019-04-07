using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.StarshipTechnologyRepair
{
    class PulseEngineRepair : ComplexRecipe
    {
        public PulseEngineRepair()
        {
            Name = "Pulse Engine (Repair)";
            Type = RecipeFilterLabels.StarshipTechRepair;
            ChildRecipes.Add(new HermeticSeal(), 1);
            ChildRecipes.Add(new MetalPlating(), 1);
        }
    }
}
