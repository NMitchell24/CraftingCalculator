using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.EnhancedGasProduct
{
    class NitrogenSalt : Recipe
    {
        public NitrogenSalt()
        {
            Name = "Nitrogen Salt";
            Type = "Enhanced Gas Product";
            Ingredients.Add(IngredientType.NITROGEN, 250);
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 50);
        }
    }
}
