using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseTerminals
{
    class ExocraftTerminal : Recipe
    {
        public ExocraftTerminal()
        {
            Name = "Exocraft Terminal";
            Type = "Base Terminal";
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.DIHYDROGEN, 25);
        }
    }
}
