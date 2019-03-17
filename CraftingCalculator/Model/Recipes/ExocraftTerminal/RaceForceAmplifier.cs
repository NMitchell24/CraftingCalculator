using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
{
    class RaceForceAmplifier : ComplexRecipe
    {
        public RaceForceAmplifier()
        {
            Name = "Race Force Amplifier";
            Type = "Exocraft Race Module";
            ChildRecipes.Add(new MetalPlating(), 5);
            ChildRecipes.Add(new DihydrogenJelly(), 2);
        }
    }
}
