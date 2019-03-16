using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.AdvancedAgriculturalProduct
{
    class Lubricant : Recipe
    {
        public Lubricant()
        {
            Name = "Lubricant";
            Type = "Advanced Agricultural Product";
            Ingredients.Add(IngredientType.COPRITE, 50);
            Ingredients.Add(IngredientType.GAMMA_ROOT, 400);
        }
    }
}
