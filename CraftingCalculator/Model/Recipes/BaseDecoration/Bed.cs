using CraftingCalculator.Model.Ingredients;

namespace CraftingCalculator.Model.Recipes.BaseDecoration
{
    class Bed : Recipe
    {
        public Bed()
        {
            Name = "Bed";
            Type = "Base Decoration";
            Ingredients.Add(IngredientType.CONDENSED_CARBON, 10);
        }
    }
}
