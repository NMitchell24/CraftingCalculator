using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnologyRepair
{
    class TerrainManipulatorRepair : ComplexRecipe
    {
        public TerrainManipulatorRepair()
        {
            Name = "Terrain Manipulator (Repair)";
            Type = RecipeFilterLabels.MultitoolTechRepair;
            ChildRecipes.Add(new CarbonNanotubes(), 1);
            ChildRecipes.Add(new DihydrogenJelly(), 1);
        }
    }
}
