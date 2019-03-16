using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.EnhancedGasProduct
{
    class EnrichedCarbon : Recipe
    {
        public EnrichedCarbon()
        {
            Name = "Enriched Carbon";
            Type = "Enhanced Gas Product";
            Ingredients.Add(IngredientType.RADON, 250);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 50);
        }
    }
}
