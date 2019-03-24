using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class Glass : Recipe
    {
        public Glass()
        {
            Name = "Glass";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            Ingredients.Add(IngredientType.FROST_CRYSTAL, 50);
        }
    }
}
