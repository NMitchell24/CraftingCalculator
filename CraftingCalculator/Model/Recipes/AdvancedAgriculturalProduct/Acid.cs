using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class Acid : Recipe
    {
        public Acid()
        {
            Name = "Acid";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            Ingredients.Add(IngredientType.MORDITE, 25);
            Ingredients.Add(IngredientType.FUNGAL_MOULD, 600);
        }
    }
}
