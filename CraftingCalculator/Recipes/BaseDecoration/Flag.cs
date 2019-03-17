using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseDecoration
{
    class Flag : Recipe
    {
        public Flag()
        {
            Name = "Flag (All)";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.PURE_FERRITE, 5);
            Ingredients.Add(IngredientType.CARBON, 5);
        }
    }
}
