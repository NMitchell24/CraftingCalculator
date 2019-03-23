using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.EnhancedGasProduct
{
    class ThermicCondensate : Recipe
    {
        public ThermicCondensate()
        {
            Name = "Thermic Condensate";
            Type = RecipeFilterLabels.EnhancedGasProduct;
            Ingredients.Add(IngredientType.SULPHURINE, 250);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 50);
        }
    }
}
