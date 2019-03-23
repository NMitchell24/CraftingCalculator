using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.PortableTechnology
{
    class SavePoint : ComplexRecipe
    {
        public SavePoint()
        {
            Name = "Save Point";
            Type = RecipeFilterLabels.PortableTech;
            ChildRecipes.Add(new MetalPlating(), 2);
            ChildRecipes.Add(new DihydrogenJelly(), 1);
        }
    }
}
