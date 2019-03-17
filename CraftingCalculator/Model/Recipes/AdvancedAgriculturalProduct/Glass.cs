using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class Glass : Recipe
    {
        public Glass()
        {
            Name = "Glass";
            Type = "Advanced Agricultural Product";
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 50);
        }
    }
}
