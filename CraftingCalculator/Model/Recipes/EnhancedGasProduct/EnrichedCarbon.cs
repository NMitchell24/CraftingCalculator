using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.EnhancedGasProduct
{
    class EnrichedCarbon : Recipe
    {
        public EnrichedCarbon()
        {
            Name = "Enriched Carbon";
            Type = RecipeFilterLabels.EnhancedGasProduct;
            Ingredients.Add(IngredientType.RADON, 250);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 50);
        }
    }
}
