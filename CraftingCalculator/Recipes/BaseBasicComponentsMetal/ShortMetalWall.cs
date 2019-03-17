using CraftingCalculator.Ingredients;

namespace CraftingCalculator.Recipes.BaseBasicComponentsMetal
{
    class ShortMetalWall : Recipe
    {
        public ShortMetalWall()
        {
            Name = "Short Metal Wall";
            Type = "Base Building";
            Ingredients.Add(IngredientType.CARBON, 20);
        }
    }
}
