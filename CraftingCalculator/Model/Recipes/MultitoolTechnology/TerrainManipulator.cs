using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.MultitoolTechnology
{
    class TerrainManipulator : ComplexRecipe
    {
        public TerrainManipulator()
        {
            Name = "Terrain Manipulator";
            Type = RecipeFilterLabels.MultitoolTech;
            ChildRecipes.Add(new DihydrogenJelly(), 1);
            ChildRecipes.Add(new CarbonNanotubes(), 2);
        }
    }
}
