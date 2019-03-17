using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseStructures
{
    class InteriorStairs : Recipe
    {
        public InteriorStairs()
        {
            Name = "Interior Stairs";
            Type = "Base Building";
            Ingredients.Add(IngredientType.PURE_FERRITE, 50);
            Ingredients.Add(IngredientType.CARBON, 30);
        }
    }
}
