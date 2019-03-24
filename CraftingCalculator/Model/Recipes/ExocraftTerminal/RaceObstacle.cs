using CraftingCalculator.Model.Recipes.CraftingComponent;

namespace CraftingCalculator.Model.Recipes.ExocraftTerminal
{
    class RaceObstacle : ComplexRecipe
    {
        public RaceObstacle()
        {
            Name = "Race Obstacle";
            Type = RecipeFilterLabels.ExocraftTerminals;
            ChildRecipes.Add(new MetalPlating(), 5);
        }
    }
}
