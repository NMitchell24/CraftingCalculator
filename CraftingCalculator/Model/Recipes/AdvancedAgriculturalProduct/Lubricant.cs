using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.AdvancedAgriculturalProduct
{
    class Lubricant : Recipe
    {
        public Lubricant()
        {
            Name = "Lubricant";
            Type = RecipeFilterLabels.AdvancedAgriculturalProduct;
            Ingredients.Add(IngredientType.COPRITE, 50);
            Ingredients.Add(IngredientType.GAMMA_ROOT, 400);
        }
    }
}
