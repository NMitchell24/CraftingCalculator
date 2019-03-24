using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.StarshipTechnology
{
    class ConflictScanner : Recipe
    {
        public ConflictScanner()
        {
            Name = "Conflict Scanner";
            Type = RecipeFilterLabels.StarshipTech;
            Ingredients.Add(IngredientType.WALKER_BRAIN, 1);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
