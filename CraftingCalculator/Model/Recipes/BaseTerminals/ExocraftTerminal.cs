using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseTerminals
{
    class ExocraftTerminal : Recipe
    {
        public ExocraftTerminal()
        {
            Name = "Exocraft Terminal";
            Type = RecipeFilterLabels.BaseTerminals;
            Ingredients.Add(IngredientType.CHROMATIC_METAL, 40);
            Ingredients.Add(IngredientType.DIHYDROGEN, 25);
        }
    }
}
