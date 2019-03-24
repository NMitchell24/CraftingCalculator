using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.ExocraftTechnology
{
    class ExocraftMountedCannon : Recipe
    {
        public ExocraftMountedCannon()
        {
            Name = "Exocraft Mounted Cannon";
            Type = RecipeFilterLabels.ExocraftTech;
            Ingredients.Add(IngredientType.PUGNEUM, 50);
            Ingredients.Add(IngredientType.COPPER, 100);
        }
    }
}
