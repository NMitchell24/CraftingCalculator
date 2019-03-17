using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.StarshipTechnology
{
    class ConflictScanner : Recipe
    {
        public ConflictScanner()
        {
            Name = "Conflict Scanner";
            Type = "Starship Scanner Module";
            Ingredients.Add(IngredientType.WALKER_BRAIN, 1);
            Ingredients.Add(IngredientType.TECHNOLOGY_MODULE, 1);
        }
    }
}
